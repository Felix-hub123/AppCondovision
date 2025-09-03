using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{
    [Authorize(Roles = "CompanyAdmin,CondoManager,FractionOwner")]
    public class FractionOwnerController : Controller
    {
        private readonly IFractionOwnerRepository _fractionOwnerRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IAssemblyRepository _assemblyRepository;
        private readonly IOccurrenceRepository _occurrenceRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FractionOwnerController> _logger;

        public FractionOwnerController(
            IFractionOwnerRepository fractionOwnerRepository,
            IUnitRepository unitRepository,
            IAssemblyRepository assemblyRepository,
            IOccurrenceRepository occurrenceRepository,
            INotificationRepository notificationRepository,
            IPaymentRepository paymentRepository,
            ICondominiumRepository condominiumRepository,
            IConverterHelper converterHelper,
            UserManager<User> userManager,
            ILogger<FractionOwnerController> logger)
        {
            _fractionOwnerRepository = fractionOwnerRepository;
            _unitRepository = unitRepository;
            _assemblyRepository = assemblyRepository;
            _occurrenceRepository = occurrenceRepository;
            _notificationRepository = notificationRepository;
            _paymentRepository = paymentRepository;
            _condominiumRepository = condominiumRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: FractionOwner/Index (Admin/Gerente)
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwners = await _fractionOwnerRepository.GetAllActiveAsync(user.CompanyId);
                var viewModels = _converterHelper.ToFractionOwnerViewModelList(fractionOwners);
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de proprietários de fração.");
                return View(new List<FractionOwnerViewModel>());
            }
        }

        // GET: FractionOwner/MyAccount (Condômino)
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> MyAccount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId);
                if (fractionOwner == null) return NotFound("Você não está associado a uma fração.");

                var viewModel = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar conta do condômino.");
                return View(new FractionOwnerViewModel());
            }
        }

        // GET: FractionOwner/MyPayments (Condômino)
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> MyPayments()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId);
                if (fractionOwner == null) return NotFound("Você não está associado a uma fração.");

                var payments = await _paymentRepository.GetPaymentsByCreatorAsync(user.Id, user.CompanyId); // Usa CreatedById
                var viewModels = _converterHelper.ToViewModel(payments); // Assume IEnumerable<PaymentViewModel>
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar pagamentos do condômino.");
                return View(Enumerable.Empty<PaymentViewModel>());
            }
        }

        // GET: FractionOwner/SubmitOccurrence (Condômino)
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> SubmitOccurrence()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId);
                if (fractionOwner == null) return NotFound("Você não está associado a uma fração.");

                ViewBag.UnitId = fractionOwner.UnitId;
                return View(new CreateOccurrenceViewModel { UnitId = fractionOwner.UnitId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de ocorrência.");
                return View(new CreateOccurrenceViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> SubmitOccurrence(CreateOccurrenceViewModel model)
        {
            try
            {
                if (model == null) return BadRequest("Dados inválidos.");

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                if (ModelState.IsValid)
                {
                    var occurrence = _converterHelper.ToEntity(model);
                    occurrence.FractionOwnerId = (await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId))?.Id;
                    occurrence.Status = "Aberta";
                    occurrence.WasDeleted = false;
                    await _occurrenceRepository.AddAsync(occurrence);
                    TempData["SuccessMessage"] = "Ocorrência submetida com sucesso!";
                    return RedirectToAction("MyAccount");
                }

                ViewBag.UnitId = model.UnitId;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao submeter ocorrência.");
                ModelState.AddModelError("", $"Erro ao submeter ocorrência: {ex.Message}");
                ViewBag.UnitId = model.UnitId;
                return View(model);
            }
        }

        // GET: FractionOwner/MyAssemblies (Condômino)
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> MyAssemblies()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId);
                if (fractionOwner == null) return NotFound("Você não está associado a uma fração.");

                var assemblies = await _assemblyRepository.GetAssembliesByCondominiumIdAsync(fractionOwner.Unit!.CondominiumId, user.CompanyId);
                var viewModels = _converterHelper.ToViewModel(assemblies); 
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar assembleias do condômino.");
                return View(Enumerable.Empty<AssemblyViewModel>());
            }
        }

        // GET: FractionOwner/MyNotifications (Condômino)
        [Authorize(Roles = "FractionOwner")]
        public async Task<IActionResult> MyNotifications()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByUserIdAsync(user.Id, user.CompanyId);
                if (fractionOwner == null) return NotFound("Você não está associado a uma fração.");

                var notifications = await _notificationRepository.GetNotificationsByCondominiumIdAsync(fractionOwner.Unit!.CondominiumId, user.CompanyId);
                var viewModels = _converterHelper.ToViewModel(notifications); // Assume IEnumerable<NotificationViewModel>
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar notificações do condômino.");
                return View(Enumerable.Empty<NotificationViewModel>());
            }
        }

        // GET: FractionOwner/Details (Admin/Gerente)
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID inválido.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id, user.CompanyId);
                if (fractionOwner == null || fractionOwner.WasDeleted)
                {
                    return NotFound();
                }

                var viewModel = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do proprietário de fração {FractionOwnerId}", id);
                return NotFound();
            }
        }

        // GET: FractionOwner/Create (Admin/Gerente)
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var activeUnits = await _unitRepository.GetAllActiveAsync(user.CompanyId);
                if (activeUnits == null || !activeUnits.Any())
                {
                    ModelState.AddModelError(string.Empty, "Nenhuma unidade ativa disponível.");
                }
                ViewBag.Units = new SelectList(activeUnits ?? new List<Unit>(), "Id", "UnitName");

                var users = await _userManager.Users.Where(u => u.CompanyId == user.CompanyId).ToListAsync();
                if (users == null || !users.Any())
                {
                    ModelState.AddModelError(string.Empty, "Nenhum usuário disponível.");
                }
                ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de proprietário de fração.");
                ModelState.AddModelError(string.Empty, "Erro ao carregar o formulário.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FractionOwnerViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Dados inválidos.");
                }

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) return RedirectToAction("Login", "Account");

                    var fractionOwner = _converterHelper.ToFractionOwner(model);
                    fractionOwner.CompanyId = user.CompanyId;
                    await _fractionOwnerRepository.CreateWithRelationsAsync(fractionOwner);
                    return RedirectToAction(nameof(Index));
                }

                var currentUser = await _userManager.GetUserAsync(User); 
                var activeUnits = await _unitRepository.GetAllActiveAsync(currentUser!.CompanyId);
                ViewBag.Units = new SelectList(activeUnits ?? new List<Unit>(), "Id", "UnitName");
                var companyId = currentUser.CompanyId; 
                var users = await _userManager.Users.Where(u => u.CompanyId == companyId).ToListAsync();
                ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar proprietário de fração.");
                ModelState.AddModelError(string.Empty, $"Erro ao criar proprietário: {ex.Message}");
                var user = await _userManager.GetUserAsync(User);
                var activeUnits = await _unitRepository.GetAllActiveAsync(user!.CompanyId);
                ViewBag.Units = new SelectList(activeUnits ?? new List<Unit>(), "Id", "UnitName");
                var companyId = user.CompanyId; 
                var users = await _userManager.Users.Where(u => u.CompanyId == companyId).ToListAsync();
                ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");
                return View(model);
            }
        }

        // GET: FractionOwner/Edit (Admin/Gerente)
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id, user.CompanyId);
                if (fractionOwner == null || fractionOwner.WasDeleted)
                {
                    return NotFound();
                }

                var viewModel = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
                var activeUnits = await _unitRepository.GetAllActiveAsync(user.CompanyId);
                ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName", viewModel.UnitId);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição para proprietário de fração {FractionOwnerId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FractionOwnerViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    return BadRequest("IDs inconsistentes.");
                }

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) return RedirectToAction("Login", "Account");

                    var fractionOwner = _converterHelper.ToFractionOwner(viewModel);
                    fractionOwner.CompanyId = user.CompanyId;
                    await _fractionOwnerRepository.UpdateWithRelationsAsync(fractionOwner);
                    return RedirectToAction(nameof(Index));
                }

                var currentUser = await _userManager.GetUserAsync(User); 
                var activeUnits = await _unitRepository.GetAllActiveAsync(currentUser!.CompanyId);
                ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName", viewModel.UnitId);
                var companyId = currentUser.CompanyId; 
                var users = await _userManager.Users.Where(u => u.CompanyId == companyId).ToListAsync();
                ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar proprietário de fração {FractionOwnerId}", id);
                ModelState.AddModelError(string.Empty, "Erro ao salvar alterações.");
                var currentUser = await _userManager.GetUserAsync(User); 
                var activeUnits = await _unitRepository.GetAllActiveAsync(currentUser!.CompanyId);
                ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName", viewModel.UnitId);
                var companyId = currentUser.CompanyId; 
                var users = await _userManager.Users.Where(u => u.CompanyId == companyId).ToListAsync();
                ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");
                return View(viewModel);
            }
        }

        // GET: FractionOwner/Delete (Admin/Gerente)
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID inválido.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id, user.CompanyId);
                if (fractionOwner == null)
                {
                    return NotFound();
                }

                var model = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar confirmação de exclusão para proprietário de fração {FractionOwnerId}", id);
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID inválido.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id, user.CompanyId);
                if (fractionOwner == null)
                {
                    return NotFound();
                }

                fractionOwner.WasDeleted = true;
                await _fractionOwnerRepository.UpdateAsync(fractionOwner);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir proprietário de fração {FractionOwnerId}", id);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

