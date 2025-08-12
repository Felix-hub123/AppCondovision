using CondoVision.Data;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{
    public class FractionController : Controller
    {
        private readonly IFractionRepository _fractionRepository;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IConverterHelper _converterHelper;

        public FractionController(
            IFractionRepository fractionRepository,
            ICondominiumRepository condominiumRepository,
            IConverterHelper converterHelper)
        {
            _fractionRepository = fractionRepository;
            _condominiumRepository = condominiumRepository;
            _converterHelper = converterHelper;
        }


        // GET: Fraction
        public async Task<IActionResult> Index()
        {
            var fractions = await _fractionRepository.GetAllFractionsWithCondominiumAsync();
            var model = fractions.Select(f => _converterHelper.ToFractionViewModel(f)).ToList();
            return View(model);
        }

        // GET: Fraction/Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateFractionViewModel
            {
                Condominiums = await GetCondominiumsSelectList()
            };
            return View(model);
        }

        // POST: Fraction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFractionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fraction = _converterHelper.ToFraction(model);
                await _fractionRepository.AddAsync(fraction);
                return RedirectToAction(nameof(Index));
            }

            model.Condominiums = await GetCondominiumsSelectList();
            return View(model);
        }

        // GET: Fraction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraction = await _fractionRepository.GetFractionWithCondominiumAsync(id.Value);
            if (fraction == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEditFractionViewModel(fraction);
            model.Condominiums = await GetCondominiumsSelectList();
            return View(model);
        }

        // POST: Fraction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditFractionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var fraction = _converterHelper.ToFraction(model);
                await _fractionRepository.UpdateAsync(fraction);
                return RedirectToAction(nameof(Index));
            }

            model.Condominiums = await GetCondominiumsSelectList();
            return View(model);
        }

        // GET: Fraction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraction = await _fractionRepository.GetFractionWithCondominiumAsync(id.Value);
            if (fraction == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToFractionViewModel(fraction);
            return View(model);
        }


        // GET: Fraction/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var fraction = await _fractionRepository.GetFractionWithCondominiumAsync(id.Value);
            if (fraction == null)
                return NotFound();

            var model = _converterHelper.ToFractionViewModel(fraction);
            return View(model);
        }

        // POST: Fraction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(FractionViewModel model)
        {
            if (model == null || model.Id == 0)
                return NotFound();

            var fraction = await _fractionRepository.GetByIdAsync(model.Id);

            if (fraction == null)
                return NotFound();

            try
            {
                fraction.WasDeleted = true;
                await _fractionRepository.UpdateAsync(fraction);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao eliminar: {ex.Message}");
                var retryModel = _converterHelper.ToFractionViewModel(fraction);
                return View(retryModel);
            }
        }




        private async Task<IEnumerable<SelectListItem>> GetCondominiumsSelectList()
        {
            var condominiums = await _condominiumRepository.GetAllCondominiumsWithCompanyAsync();
            return condominiums.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).OrderBy(c => c.Text);
        }
    }
}
