using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyService;

        public CompanyController(ICompanyRepository companyService)
        {
            _companyService = companyService;
        }


        public async Task<IActionResult> Index()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return View(companies);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var company = await _companyService.GetCompanyByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                await _companyService.AddCompanyAsync(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyService.GetCompanyByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _companyService.UpdateCompanyAsync(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _companyService.GetCompanyByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _companyService.DeleteCompanyAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
