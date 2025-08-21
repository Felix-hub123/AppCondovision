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
        private readonly UserManager<User> _userManager;



        public CondominiumController(
            ICondominiumRepository condominiumRepository,
            ILogger<CondominiumController> logger,
            IConverterHelper converterHelper,
            ICompanyRepository companyRepository,
            UserManager<User> userManager

            )
        {
            _condominiumRepository = condominiumRepository;
            _logger = logger;
            _converterHelper = converterHelper;
            _companyRepository = companyRepository;
            _userManager = userManager;

        }


        
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? companyId)
        {
            var condominiums = await _condominiumRepository.GetAllCondominiumsWithCompanyAsync();
            if (companyId.HasValue)
            {
                condominiums = condominiums.Where(c => c.CompanyId == companyId);
            }
            var model = condominiums.Select(c => _converterHelper.ToCondominiumViewModel(c)).ToList();

            
            var companies = await _companyRepository.GetAllCompaniesAsync();
            ViewBag.Companies = companies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name 
            }).ToList();

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync(); 
            return View(new CreateCondominiumViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateCondominiumViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var condominium = _converterHelper.ToCondominium(model);
                    await _condominiumRepository.AddAsync(condominium);
                    await _condominiumRepository.CompleteAsync();

                    TempData["SuccessMessage"] = "Condomínio criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao criar o condomínio: {ex.Message}");
                    _logger.LogError(ex, "Erro ao criar condomínio");
                }
            }

            model.Companies = await GetCompaniesSelectList();
            return View(model);
        }

        [AllowAnonymous]
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

            var model = _converterHelper.ToEditCondominiumViewModelAsync(condominium).Result; 
            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(int id, EditCondominiumViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                    if (condominium != null && !condominium.WasDeleted)
                    {
                        condominium.Name = model.Name;
                        condominium.Address = model.Address;
                        condominium.City = model.City;
                        condominium.PostalCode = model.PostalCode;
                        condominium.CompanyId = model.CompanyId;
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
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao atualizar o condomínio: {ex.Message}");
                    _logger.LogError(ex, "Erro ao atualizar condomínio");
                }
            }

            model.Companies = await GetCompaniesSelectList();
            return View(model);
        }

        [AllowAnonymous]
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

            var condominiumVM = _converterHelper.ToCondominiumViewModel(condominium);
            return View(condominiumVM);
        }

        // POST: Condominium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userName = user?.Email ?? "Usuário Anônimo";

                var condominium = await _condominiumRepository.GetCondominiumByIdAsync(id);
                if (condominium != null)
                {
                    condominium.WasDeleted = true; // Soft delete
                    await _condominiumRepository.UpdateCondominiumAsync(condominium); // Ajuste o método conforme necessário
                    _logger.LogInformation("Condomínio com ID {CondominiumId} foi marcado como excluído por {UserName} às {Time} (WEST).",
                        id, userName, DateTime.Now);
                    TempData["SuccessMessage"] = "Condomínio excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de excluir condomínio com ID {Id} falhou: condomínio não encontrado.", id);
                    TempData["ErrorMessage"] = "Condomínio não encontrado.";
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

        private async Task<IEnumerable<SelectListItem>> GetCompaniesSelectList()
        {
            return (await _companyRepository.GetAllCompaniesAsync())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
        }
    }
}
