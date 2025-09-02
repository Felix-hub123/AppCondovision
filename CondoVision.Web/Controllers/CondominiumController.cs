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
    public class CondominiumController : Controller
    {
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly ILogger<CondominiumController> _logger;
        private readonly IConverterHelper _converterHelper;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly UserManager<User> _userManager;



        public CondominiumController(
              ICondominiumRepository condominiumRepository,
              ILogger<CondominiumController> logger,
              IConverterHelper converterHelper,
              ICompanyRepository companyRepository,
              IUnitRepository unitRepository,
              UserManager<User> userManager)
        {
            _condominiumRepository = condominiumRepository;
            _logger = logger;
            _converterHelper = converterHelper;
            _companyRepository = companyRepository;
            _unitRepository = unitRepository;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            var condominiums = await _condominiumRepository.GetAllCondominiumsWithCompanyAsync();
            if (companyId.HasValue)
            {
                condominiums = condominiums.Where(c => c.CompanyId == companyId);
            }
            var model = condominiums.Select(c => _converterHelper.ToCondominiumViewModel(c)).ToList();

            var companies = await _companyRepository.GetAllAsync();
            ViewBag.Companies = companies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Units(int condominiumId)
        {
            var condominium = await _condominiumRepository.GetByIdAsync(condominiumId);
            if (condominium == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (condominium.CompanyId != user?.CompanyId && !User.IsInRole("CompanyAdmin"))
            {
                return Forbid();
            }

            var units = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId);
            var model = _converterHelper.ToViewModel(units);
            ViewBag.CondominiumId = condominiumId;
            ViewBag.CondominiumName = condominium.Name;
            return View(model);
        }

        [AllowAnonymous] // ou [Authorize] dependendo do requisito
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new CreateCondominiumViewModel();

            if (user != null && user.CompanyId.HasValue)
            {
                model.Companies = User.IsInRole("CompanyAdmin")
                    ? (await _companyRepository.GetCompaniesWithCondominiumsAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    : (await _companyRepository.GetCompaniesWithCondominiumsAsync(user.CompanyId.Value)).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
            }
            else
            {
                model.Companies = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Nenhuma empresa associada" } };
                ModelState.AddModelError(string.Empty, "Utilizador sem empresa associada. Contacte um administrador.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Create(CreateCondominiumViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

           
            if (user != null && user.CompanyId.HasValue)
            {
                model.Companies = User.IsInRole("CompanyAdmin")
                    ? (await _companyRepository.GetCompaniesWithCondominiumsAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    : (await _companyRepository.GetCompaniesWithCondominiumsAsync(user.CompanyId.Value)).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
            }
            else
            {
                model.Companies = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Nenhuma empresa associada" } };
                ModelState.AddModelError(string.Empty, "Utilizador sem empresa associada. Contacte um administrador.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (user == null || !user.CompanyId.HasValue)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário não autorizado ou sem empresa associada.");
                        return View(model);
                    }

                    model.CompanyId = user.CompanyId.Value; 
                    var condominium = _converterHelper.ToCondominium(model);
                    await _condominiumRepository.AddAsync(condominium);
                    await _condominiumRepository.CompleteAsync();

                    TempData["SuccessMessage"] = "Condomínio criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados.");
                    _logger.LogError(ex, "Erro de banco de dados ao criar condomínio");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro inesperado ao criar condomínio.");
                    _logger.LogError(ex, "Erro inesperado ao criar condomínio");
                }
            }

            return View(model);
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id.Value);
            if (condominium == null || condominium.WasDeleted)
            {
                return NotFound();
            }

            if (!await IsAuthorizedForCondominiumAsync(id.Value))
                return Forbid();

            var model = await _converterHelper.ToEditCondominiumViewModelAsync(condominium);
            ViewBag.Companies = (await _companyRepository.GetAllAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == condominium.CompanyId
                }).ToList();

            if (condominium.CompanyId.HasValue)
            {
                var company = await _companyRepository.GetByIdAsync(condominium.CompanyId.Value);
                ViewBag.CompanyName = company?.Name ?? "Nenhum";
            }
            else
            {
                ViewBag.CompanyName = "Nenhum";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Edit(int id, EditCondominiumViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (model.CompanyId != user?.CompanyId && !User.IsInRole("CompanyAdmin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                    if (condominium != null && !condominium.WasDeleted)
                    {
                        _converterHelper.UpdateCondominium(condominium, model);
                        await _condominiumRepository.UpdateCondominiumAsync(condominium);
                        TempData["SuccessMessage"] = "Condomínio atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _condominiumRepository.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados.");
                    _logger.LogError(ex, "Erro de banco de dados ao atualizar condomínio");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro inesperado ao atualizar condomínio.");
                    _logger.LogError(ex, "Erro inesperado ao atualizar condomínio");
                }
            }

            model.Companies = await GetCompaniesSelectList(user!.CompanyId!.Value);
            return View(model);
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id.Value);
            if (condominium == null || condominium.WasDeleted)
            {
                return NotFound();
            }

            if (!await IsAuthorizedForCondominiumAsync(id.Value))
                return Forbid();

            var condominiumVM = _converterHelper.ToCondominiumViewModel(condominium);
            return View(condominiumVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                if (condominium != null && condominium.CompanyId == user?.CompanyId)
                {
                    condominium.WasDeleted = true;
                    await _condominiumRepository.UpdateCondominiumAsync(condominium);
                    _logger.LogInformation("Condomínio com ID {CondominiumId} foi marcado como excluído por {UserName} às {Time} (WEST).",
                        id, user?.Email ?? "Usuário Anônimo", DateTime.Now);
                    TempData["SuccessMessage"] = "Condomínio excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de excluir condomínio com ID {Id} falhou: acesso negado.", id);
                    TempData["ErrorMessage"] = "Acesso negado ou condomínio não encontrado.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco de dados ao excluir condomínio com ID {Id} por {UserName} às {Time} (WEST).",
                    id, (await _userManager.GetUserAsync(User))?.Email ?? "Usuário Anônimo", DateTime.Now);
                ModelState.AddModelError("", "Erro ao excluir: problema com o banco de dados.");
                var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                return condominium == null ? NotFound() : View("Delete", _converterHelper.ToCondominiumViewModel(condominium));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir condomínio com ID {Id} por {UserName} às {Time} (WEST).",
                    id, (await _userManager.GetUserAsync(User))?.Email ?? "Usuário Anônimo", DateTime.Now);
                ModelState.AddModelError("", "Erro inesperado ao excluir.");
                var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                return condominium == null ? NotFound() : View("Delete", _converterHelper.ToCondominiumViewModel(condominium));
            }
        }

        [Authorize(Roles = "CompanyAdmin,CondoManager")]
        public async Task<IActionResult> Details(int id)
        {
            var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
            if (condominium == null || condominium.WasDeleted)
                return NotFound();

            if (!await IsAuthorizedForCondominiumAsync(id))
                return Forbid();

            var model = _converterHelper.ToCondominiumViewModel(condominium);
            return View(model);
        }

        private async Task<bool> IsAuthorizedForCondominiumAsync(int condominiumId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.CompanyId.HasValue)
                return false;

            var condominium = await _condominiumRepository.GetCondominiumByIdAsync(condominiumId);
            return condominium != null && (condominium.CompanyId == user.CompanyId || User.IsInRole("CompanyAdmin"));
        }

        private async Task<IEnumerable<SelectListItem>> GetCompaniesSelectList(int companyId)
        {
            var companies = await _companyRepository.GetCompaniesWithCondominiumsAsync(companyId);
            return companies.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
        }
    }
}
