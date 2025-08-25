using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;
        private readonly ICondominiumRepository _condominiumRepository; // Para gerenciar condomínios

        public CompanyController(
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

        // GET: Companies/Index
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            var companies = await _companyRepository.GetCompaniesWithCondominiumsAsync();
            if (loggedUser != null)
            {
                companies = companies.Where(c => c.Id == loggedUser.CompanyId).ToList(); // Filtra pela empresa do usuário
            }
            return View(companies);
        }

        // GET: Companies/Details/5
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

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            var condominiums = await _condominiumRepository.GetCondominiumsByCompanyIdAsync(id.Value); 
            companyVM.Condominiums = _converterHelper.ToCondominiumViewModelList(condominiums); 
            return View(companyVM);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View(new CompanyViewModel());
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyViewModel companyVM)
        {
            if (ModelState.IsValid)
            {
                var company = _converterHelper.ToCompany(companyVM);
                company.CreationDate = DateTime.UtcNow;
                company.WasDeleted = false;

                // Associar usuário logado como admin da empresa
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    company.CreatedById = user.Id;
                    user.CompanyId = company.Id; // Associa o usuário à empresa
                    await _userManager.UpdateAsync(user);
                }

                await _companyRepository.CreateAsync(company);
                return RedirectToAction(nameof(Index));
            }
            return View(companyVM);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            if (company.Id != loggedUser!.CompanyId)
            {
                return Forbid(); // Restringe edição a empresas do usuário
            }

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            return View(companyVM);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyViewModel companyVM)
        {
            if (id != companyVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var company = await _companyRepository.GetByIdAsync(id);
                    if (company == null)
                    {
                        return NotFound();
                    }

                    var loggedUser = await _userManager.GetUserAsync(User);
                    if (company.Id != loggedUser!.CompanyId)
                    {
                        return Forbid();
                    }

                    _converterHelper.UpdateCompany(company, companyVM);
                    await _companyRepository.UpdateAsync(company);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _companyRepository.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(companyVM);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
            if (company.Id != loggedUser!.CompanyId)
            {
                return Forbid();
            }

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            return View(companyVM);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                if (company != null && company.Id == user!.CompanyId) // Verifica permissão
                {
                    await _companyRepository.DeleteCompanyAsync(id);
                    TempData["SuccessMessage"] = "Empresa excluída com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Permissão negada ou empresa não encontrada.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Erro ao excluir: problema com o banco de dados.");
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Erro inesperado ao excluir.");
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
        }

        // API: Get Companies
        [HttpGet("api/companies")]
        public async Task<IActionResult> GetCompaniesJson()
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            var companies = await _companyRepository.GetCompaniesWithCondominiumsAsync();
            if (loggedUser != null)
            {
                companies = companies.Where(c => c.Id == loggedUser.CompanyId).ToList();
            }
            return Json(companies);
        }

        // API: Get Company Details
        [HttpGet("api/companies/{id}")]
        public async Task<IActionResult> GetCompanyDetailsJson(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);
            if (company == null || company.WasDeleted)
            {
                return NotFound();
            }

            var loggedUser = await _userManager.GetUserAsync(User);
            if (company.Id != loggedUser!.CompanyId)
            {
                return Forbid();
            }

            var companyVM = _converterHelper.ToCompanyViewModel(company);
            var condominiums = await _condominiumRepository.GetCondominiumsByCompanyIdAsync(id); // Retorna List<Condominium>
            companyVM.Condominiums = _converterHelper.ToCondominiumViewModelList(condominiums).ToList(); // Converte para List<CondominiumViewModel>
            return Json(companyVM);
        }
    }
}
