using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentRepository paymentRepository, IUnitRepository unitRepository,
            IConverterHelper converterHelper, UserManager<User> userManager, ILogger<PaymentController> logger)
        {
            _paymentRepository = paymentRepository;
            _unitRepository = unitRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Payment/Index
        public async Task<IActionResult> Index(int? unitId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                int? companyId = user.CompanyId;
                var units = await _unitRepository.GetAllAsync(companyId) ?? new List<Unit>();
                ViewBag.Units = new SelectList(units, "Id", "UnitName", unitId);

                IEnumerable<PaymentViewModel> payments = Enumerable.Empty<PaymentViewModel>();
                if (unitId.HasValue && unitId > 0)
                {
                    var unit = await _unitRepository.GetByIdAsync(unitId.Value, companyId);
                    if (unit != null)
                    {
                        payments = _converterHelper.ToViewModel(await _paymentRepository.GetPaymentsByUnitIdAsync(unitId.Value, companyId));
                        ViewBag.UnitId = unitId;
                        ViewBag.UnitName = unit.UnitName;
                    }
                }

                return View(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar pagamentos para UnitId {UnitId}", unitId);
                ModelState.AddModelError("", $"Erro ao carregar a página: {ex.Message}");
                return View(Enumerable.Empty<PaymentViewModel>());
            }
        }

        // GET: Payment/Create
        public async Task<IActionResult> Create(int? unitId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                int? companyId = user.CompanyId;
                var unit = await _unitRepository.GetByIdAsync(unitId ?? 0, companyId);
                if (unit == null) return NotFound();

                ViewBag.UnitId = unitId;
                ViewBag.UnitName = unit.UnitName;
                return View(new CreatePaymentViewModel { UnitId = unitId ?? 0, PaymentDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de pagamento para UnitId {UnitId}", unitId);
                return BadRequest($"Erro ao carregar formulário: {ex.Message}");
            }
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePaymentViewModel model)
        {
            if (model == null || model.UnitId <= 0) return BadRequest("Dados inválidos.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var unit = await _unitRepository.GetByIdAsync(model.UnitId, user.CompanyId);
                if (unit == null) return NotFound();

                if (ModelState.IsValid)
                {
                    var payment = _converterHelper.ToEntity(model); 
                    payment.IsPaid = true;
                    payment.WasDeleted = false;
                    await _paymentRepository.AddAsync(payment);
                    TempData["SuccessMessage"] = "Pagamento registrado com sucesso!";
                    return RedirectToAction(nameof(Index), new { unitId = model.UnitId });
                }

                ViewBag.UnitId = model.UnitId;
                ViewBag.UnitName = unit.UnitName;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pagamento para UnitId {UnitId}", model.UnitId);
                ModelState.AddModelError("", $"Erro ao registrar pagamento: {ex.Message}");
                ViewBag.UnitId = model.UnitId;
                ViewBag.UnitName = (await _unitRepository.GetByIdAsync(model.UnitId, user.CompanyId))?.UnitName;
                return View(model);
            }
        }

        // GET: Payment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                int? companyId = user.CompanyId;
                var payment = await _paymentRepository.GetByIdAsync(id, companyId);
                if (payment == null) return NotFound();

                var model = _converterHelper.ToViewModel(payment);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do pagamento {PaymentId}", id);
                return NotFound($"Erro ao carregar detalhes: {ex.Message}");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CondoManager")]
        public async Task<IActionResult> Validate(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                int? companyId = user.CompanyId;
                var payment = await _paymentRepository.GetByIdAsync(id, companyId);
                if (payment == null) return NotFound();

                payment.IsPaid = true; // Marca como pago
                payment.ValidatedById = user.Id; // Rastreia quem validou
                payment.ValidationDate = DateTime.Now;
                await _paymentRepository.UpdateAsync(payment);
                TempData["SuccessMessage"] = "Pagamento validado com sucesso!";
                return RedirectToAction(nameof(Index), new { unitId = payment.UnitId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar pagamento {PaymentId}", id);
                ModelState.AddModelError("", $"Erro ao validar pagamento: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
