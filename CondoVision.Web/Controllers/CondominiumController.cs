using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{
    public class CondominiumController : Controller
    {
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly ICompanyRepository _companyRepository;

        public CondominiumController(
            ICondominiumRepository condominiumRepository,
            ICompanyRepository companyRepository
            )
        {
            _condominiumRepository = condominiumRepository;
            _companyRepository = companyRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var condominiums = await _condominiumRepository.GetAllCondominiumsWithCompanyAsync();
            return View(condominiums);
        }

        // Ação para exibir os detalhes de um condomínio
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = await _condominiumRepository.GetCondominiumWithCompanyAsync(id.Value);
            if (condominium == null)
                return NotFound();

            return View(condominium);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Condominium condominium)
        {
            if (ModelState.IsValid)
            {
                condominium.CreationDate = DateTime.UtcNow;
                condominium.WasDeleted = false;
                await _condominiumRepository.AddAsync(condominium);
                await _condominiumRepository.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync();
            return View(condominium);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = await _condominiumRepository.GetCondominiumWithCompanyAsync(id.Value);
            if (condominium == null)
                return NotFound();

            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync();
            return View(condominium);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Condominium condominium)
        {
            if (id != condominium.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _condominiumRepository.UpdateAsync(condominium);
                await _condominiumRepository.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Companies = await _companyRepository.GetAllCompaniesAsync();
            return View(condominium);
        }

        // Ação para exibir a página de confirmação de eliminação
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = await _condominiumRepository.GetCondominiumWithCompanyAsync(id.Value);
            if (condominium == null)
                return NotFound();

            return View(condominium);
        }

        // Ação POST para processar a eliminação (soft-delete) de um condomínio
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _condominiumRepository.DeleteAsync(id);
            await _condominiumRepository.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

       

    }
}
