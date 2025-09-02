using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{
    public class UnitController : Controller
    {

        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UnitController> _logger;

        public UnitController(
              IUnitRepository unitRepository,
              IUserRepository userRepository,
              ICondominiumRepository condominiumRepository,
              IConverterHelper converterHelper,
              UserManager<User> userManager,
              ILogger<UnitController> logger)
        {
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _condominiumRepository = condominiumRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Index(int? condominiumId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                int? companyId = user.CompanyId;
                var condominiums = await _condominiumRepository.GetAllActiveAsync(companyId) ?? new List<Condominium>();
                ViewBag.Condominiums = new SelectList(condominiums, "Id", "Name", condominiumId);

                IEnumerable<UnitViewModel> units = Enumerable.Empty<UnitViewModel>();
                if (condominiumId.HasValue && condominiumId > 0)
                {
                    var condo = await _condominiumRepository.GetByIdAsync(condominiumId.Value, companyId);
                    if (condo != null)
                    {
                        var unitEntities = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId.Value, companyId);
                        units = _converterHelper.ToViewModel(unitEntities) ?? Enumerable.Empty<UnitViewModel>();
                        ViewBag.CondominiumId = condominiumId;
                        ViewBag.CondominiumName = condo.Name;
                        _logger.LogInformation("Unidades recuperadas para CondominiumId {CondominiumId}: {UnitCount}", condominiumId, units.Count()); // Log para depuração
                    }
                }

                return View(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar unidades para CondominiumId {CondominiumId}", condominiumId);
                ModelState.AddModelError("", $"Erro ao carregar a página: {ex.Message}");
                return View(Enumerable.Empty<UnitViewModel>());
            }
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest("ID da unidade inválido.");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var unit = await _unitRepository.GetByIdAsync(id, user.CompanyId);
                if (unit == null) return NotFound();

                var condo = await _condominiumRepository.GetByIdAsync(unit.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                ViewBag.CondominiumId = unit.CondominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(_converterHelper.ToViewModel(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da unidade {UnitId}", id);
                ModelState.AddModelError("", $"Erro ao carregar detalhes: {ex.Message}");
                return View(new UnitViewModel());
            }
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Create(int condominiumId)
        {
            if (condominiumId <= 0) return BadRequest("ID do condomínio inválido.");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var condo = await _condominiumRepository.GetByIdAsync(condominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = condominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(new UnitViewModel { CondominiumId = condominiumId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar criação de unidade para CondominiumId {CondominiumId}", condominiumId);
                return BadRequest($"Erro ao carregar a página: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Create(UnitViewModel model)
        {
            if (model == null || model.CondominiumId <= 0) return BadRequest("Dados inválidos.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var condo = await _condominiumRepository.GetByIdAsync(model.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                if (ModelState.IsValid)
                {
                    var unit = _converterHelper.ToEntity(model);
                    unit.WasDeleted = false; // Garantir que a nova unidade não seja marcada como deletada
                    await _unitRepository.AddAsync(unit);
                    TempData["SuccessMessage"] = "Unidade criada com sucesso!";
                    return RedirectToAction(nameof(Index), new { condominiumId = model.CondominiumId });
                }

                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = model.CondominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar unidade para CondominiumId {CondominiumId}", model.CondominiumId);
                ModelState.AddModelError("", $"Erro ao criar unidade: {ex.Message}");
                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = model.CondominiumId;
                ViewBag.CondominiumName = (await _condominiumRepository.GetByIdAsync(model.CondominiumId, user.CompanyId))?.Name;
                return View(model);
            }
        }


        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest("ID da unidade inválido.");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var unit = await _unitRepository.GetByIdAsync(id, user.CompanyId);
                if (unit == null) return NotFound();

                var condo = await _condominiumRepository.GetByIdAsync(unit.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = unit.CondominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(_converterHelper.ToEditUnitViewModel(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar edição de unidade {UnitId}", id);
                return BadRequest($"Erro ao carregar edição: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Edit(int id, EditUnitViewModel model)
        {
            if (id != model.Id || model.CondominiumId <= 0) return BadRequest("IDs inconsistentes.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var condo = await _condominiumRepository.GetByIdAsync(model.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                if (ModelState.IsValid)
                {
                    var unit = await _unitRepository.GetByIdAsync(id, user.CompanyId);
                    if (unit == null) return NotFound();

                    _converterHelper.UpdateUnit(unit, model); 
                    await _unitRepository.UpdateAsync(unit);
                    TempData["SuccessMessage"] = "Unidade atualizada com sucesso!";
                    return RedirectToAction(nameof(Index), new { condominiumId = model.CondominiumId });
                }

                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = model.CondominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar unidade {UnitId}", id);
                ModelState.AddModelError("", $"Erro ao editar unidade: {ex.Message}");
                ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(user.CompanyId), "Id", "FullName");
                ViewBag.CondominiumId = model.CondominiumId;
                ViewBag.CondominiumName = (await _condominiumRepository.GetByIdAsync(model.CondominiumId, user.CompanyId))?.Name;
                return View(model);
            }
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("ID da unidade inválido.");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var unit = await _unitRepository.GetByIdAsync(id, user.CompanyId);
                if (unit == null) return NotFound();

                var condo = await _condominiumRepository.GetByIdAsync(unit.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                ViewBag.CondominiumId = unit.CondominiumId;
                ViewBag.CondominiumName = condo.Name;
                return View(_converterHelper.ToViewModel(unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar exclusão de unidade {UnitId}", id);
                return BadRequest($"Erro ao carregar exclusão: {ex.Message}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0) return BadRequest("ID da unidade inválido.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var unit = await _unitRepository.GetByIdAsync(id, user.CompanyId);
                if (unit == null) return NotFound();

                var condo = await _condominiumRepository.GetByIdAsync(unit.CondominiumId, user.CompanyId);
                if (condo == null) return Forbid();

                unit.WasDeleted = true;
                await _unitRepository.UpdateAsync(unit);
                _logger.LogInformation("Unidade {UnitId} marcada como excluída por {UserName} às {Time} (WEST)",
                    id, user.Email, DateTime.Now);
                TempData["SuccessMessage"] = "Unidade marcada como excluída com sucesso!";
                return RedirectToAction(nameof(Index), new { condominiumId = unit.CondominiumId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao eliminar unidade {UnitId} por {UserName} às {Time} (WEST)",
                    id, user.Email, DateTime.Now);
                ModelState.AddModelError("", $"Erro ao eliminar unidade: {ex.Message}");
                return View(_converterHelper.ToViewModel(await _unitRepository.GetByIdAsync(id, user.CompanyId)));
            }
        }

        [HttpGet]
        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> GetUnitsByCondominiumId(int condominiumId)
        {
            if (condominiumId <= 0) return Json(new { success = false, message = "Condominium ID inválido." });

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Json(new { success = false, message = "Usuário não autenticado." });

                var condo = await _condominiumRepository.GetByIdAsync(condominiumId, user.CompanyId);
                if (condo == null) return Json(new { success = false, message = "Condomínio não encontrado ou sem permissão." });

                var units = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId, user.CompanyId);
                var unitModels = _converterHelper.ToViewModel(units);
                _logger.LogInformation("Unidades carregadas para CondominiumId {CondominiumId} por {UserName}", condominiumId, user.Email);
                return Json(new { success = true, units = unitModels });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar unidades para CondominiumId {CondominiumId}", condominiumId);
                return Json(new { success = false, message = $"Erro ao carregar unidades: {ex.Message}" });
            }
        }


    }
}
