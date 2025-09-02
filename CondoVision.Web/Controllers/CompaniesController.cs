using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;
        private readonly ICondominiumRepository _condominiumRepository;

        public CompaniesController(
               ICompanyRepository companyRepository,
               IConverterHelper converterHelper,
               UserManager<User> userManager,
               ICondominiumRepository condominiumRepository)
        {
            _companyRepository = companyRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
            _condominiumRepository = condominiumRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var loggedUser = await _userManager.GetUserAsync(User);
                if (loggedUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var companyId = loggedUser.CompanyId;
                IEnumerable<CompanyViewModel> model;
                if (!User.IsInRole("CompanyAdmin") && companyId.HasValue && companyId.Value > 0)
                {
                    var company = await _companyRepository.GetCompanyByIdAsync(companyId.Value);
                    if (company == null || company.WasDeleted)
                    {
                        return NotFound();
                    }
                    model = new List<CompanyViewModel> { _converterHelper.ToCompanyViewModel(company) };
                }
                else
                {
                    var companies = await _companyRepository.GetCompaniesWithCondominiumsAsync();
                    model = companies.Select(c => _converterHelper.ToCompanyViewModel(c));
                }
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao carregar empresas. Tente novamente mais tarde.";
                return View(new List<CompanyViewModel>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyRepository.GetCompanyByIdAsync(id.Value);
            if (company == null || company.WasDeleted)
            {
                return NotFound();

            }
            var loggedUser = await _userManager.GetUserAsync(User);
            if (company.Id != loggedUser?.CompanyId && !User.IsInRole("CompanyAdmin")) return Forbid();

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            var condominiums = await _condominiumRepository.GetCondominiumsByCompanyIdAsync(id.Value);
            companyVM.Condominiums = _converterHelper.ToCondominiumViewModelList(condominiums).ToList();
            return View(companyVM);
        }

        [Authorize(Roles = "CompanyAdmin")]
        public IActionResult Create()
        {
            return View(new CompanyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Create(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   
                    if (!await _companyRepository.IsTaxIdUniqueAsync(model.CompanyTaxId!))
                    {
                        ModelState.AddModelError("CompanyTaxId", "O NIF/NIPC já está em uso por outra empresa.");
                        return View(model);
                    }

                    var company = _converterHelper.ToCompany(model);
                    company.WasDeleted = false;
                    company.CreationDate = DateTime.Now;

                    await _companyRepository.AddCompanyAsync(company);
                    TempData["SuccessMessage"] = "Empresa criada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException )
                {
                    ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados.");
                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Erro inesperado ao criar empresa.");
                  
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Edit(int id, CompanyViewModel companyVM)
        {
            if (id != companyVM.Id) return NotFound();


            if (string.IsNullOrEmpty(companyVM.CompanyTaxId))
            {
                ModelState.AddModelError("CompanyTaxId", "O número de identificação fiscal é obrigatório.");
                return View(companyVM);
            }


            if (!await _companyRepository.IsTaxIdUniqueAsync(companyVM.CompanyTaxId, id))
            {
                ModelState.AddModelError("CompanyTaxId", "O número de identificação fiscal já está em uso.");
                return View(companyVM);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var company = await _companyRepository.GetCompanyByIdAsync(id);
                    if (company == null) return NotFound();

                    var loggedUser = await _userManager.GetUserAsync(User);
                    if (company.Id != loggedUser?.CompanyId) return Forbid();

                    _converterHelper.UpdateCompany(company, companyVM);
                    await _companyRepository.UpdateCompanyAsync(company);
                    TempData["SuccessMessage"] = "Empresa atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var updatedCompany = await _companyRepository.GetCompanyByIdAsync(id);
                    if (updatedCompany == null) return NotFound();
                    throw;
                }
            }
            return View(companyVM);
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> ManageUsers(int? companyId)
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            if (loggedUser == null || !loggedUser.CompanyId.HasValue)
            {
                return NotFound("Utilizador sem empresa associada.");
            }

            companyId = companyId ?? loggedUser.CompanyId;

            var company = await _companyRepository.GetCompanyByIdAsync(companyId.Value);
            if (company == null || company.WasDeleted)
            {
                return NotFound("Empresa não encontrada ou eliminada.");
            }

            if (companyId != loggedUser.CompanyId)
            {
                return Forbid("Acesso negado a esta empresa.");
            }

            var users = await _userManager.Users
                .Where(u => u.CompanyId == companyId && !u.WasDeleted)
                .ToListAsync();

            var model = new ManageUsersViewModel
            {
                CompanyId = companyId.Value,
                Users = new List<UserRoleViewModel>()
            };

            if (users.Any())
            {
                foreach (var u in users)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    model.Users.Add(new UserRoleViewModel
                    {
                        UserId = u.Id,
                        Email = u.Email,
                        Roles = roles.ToList(),
                        AssignedCompanyId = u.CompanyId
                    });
                }
            }
            else
            {
                ViewBag.Message = "Nenhum utilizador associado a esta empresa. <a href='/Account/Create'>Crie o primeiro utilizador aqui</a>.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> AssignRole(int companyId, string userId, string role)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(companyId);
            if (company == null || company.WasDeleted) return NotFound();

            var loggedUser = await _userManager.GetUserAsync(User);
            if (company.Id != loggedUser?.CompanyId) return Forbid();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.CompanyId != companyId) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

            var allowedRoles = new[] { "CompanyAdmin", "CondoManager", "Condómino" };
            if (allowedRoles.Contains(role))
            {
                await _userManager.AddToRoleAsync(user, role);
                if (user.CompanyId != companyId) user.CompanyId = companyId;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                return BadRequest("Role inválida.");
            }

            TempData["SuccessMessage"] = $"Role '{role}' atribuída ao usuário {user.Email} com sucesso!";
            return RedirectToAction(nameof(ManageUsers), new { companyId });
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = await _companyRepository.GetCompanyByIdAsync(id.Value);
            if (company == null || company.WasDeleted) return NotFound();

            var loggedUser = await _userManager.GetUserAsync(User);
            if (company.Id != loggedUser?.CompanyId) return Forbid();

            return View(_converterHelper.ToCompanyViewModel(company));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                if (company == null || company.WasDeleted || company.Id != user?.CompanyId)
                {
                    TempData["ErrorMessage"] = "Permissão negada ou empresa não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                await _companyRepository.DeleteCompanyAsync(id);
                TempData["SuccessMessage"] = "Empresa marcada como excluída com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir: {ex.Message}";
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro inesperado ao excluir: {ex.Message}";
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
        }

        [HttpGet("api/companies")]
        public async Task<IActionResult> GetCompaniesJson()
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            var companies = loggedUser != null
                ? await _companyRepository.GetCompaniesWithCondominiumsAsync(loggedUser.CompanyId)
                : await _companyRepository.GetCompaniesWithCondominiumsAsync();
            return Json(_converterHelper.ToCompanyViewModelList(companies));
        }

        [HttpGet("api/companies/{id}")]
        public async Task<IActionResult> GetCompanyDetailsJson(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);
            if (company == null) return NotFound();

            var loggedUser = await _userManager.GetUserAsync(User);
            if (company.Id != loggedUser?.CompanyId) return Forbid();

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            var condominiums = await _condominiumRepository.GetCondominiumsByCompanyIdAsync(id);
            companyVM.Condominiums = _converterHelper.ToCondominiumViewModelList(condominiums).ToList();
            return Json(companyVM);
        }
    }
}
