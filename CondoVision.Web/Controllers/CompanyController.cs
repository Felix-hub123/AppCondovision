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
        private readonly ILogger<CompanyController> _logger;
        private readonly UserManager<User> _userManager;

        public CompanyController(
            ICompanyRepository companyRepository,
            IConverterHelper converterHelper,
            ILogger<CompanyController> logger,
            UserManager<User> userManager
            )
        {
            _companyRepository = companyRepository;
            _converterHelper = converterHelper;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _companyRepository.GetCompaniesWithCondominiumsAsync();
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
                await _companyRepository.CreateAsync(company);
                return RedirectToAction(nameof(Index));
            }
            return View(companyVM);
        }

      

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
                    if (company != null)
                    {
                        _converterHelper.UpdateCompany(company, companyVM);
                        await _companyRepository.UpdateAsync(company);
                    }
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
                var userName = user?.Email ?? "Usuário Anônimo";

                var company = await _companyRepository.GetCompanyByIdAsync(id); 
                if (company != null)
                {
                    await _companyRepository.DeleteCompanyAsync(id); 
                    _logger.LogInformation("Empresa com ID {CompanyId} foi marcada como excluída por {UserName} às {Time} (WEST).",
                        id, userName, DateTime.Now);
                    TempData["SuccessMessage"] = "Empresa excluída com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de excluir empresa com ID {Id} falhou: empresa não encontrada.", id);
                    TempData["ErrorMessage"] = "Empresa não encontrada.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco de dados ao excluir empresa com ID {Id} por {UserName} às {Time} (WEST).",
                    id, (await _userManager.GetUserAsync(User))?.Email ?? "Usuário Anônimo", DateTime.Now);
                ModelState.AddModelError("", "Erro ao excluir: problema com o banco de dados.");
       
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir empresa com ID {Id} por {UserName} às {Time} (WEST).",
                    id, (await _userManager.GetUserAsync(User))?.Email ?? "Usuário Anônimo", DateTime.Now);
                ModelState.AddModelError("", "Erro inesperado ao excluir.");
                var company = await _companyRepository.GetCompanyByIdAsync(id);
                return company == null ? NotFound() : View("Delete", _converterHelper.ToCompanyViewModel(company));
            }
        }
    }
}
