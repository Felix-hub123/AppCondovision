using CondoVision.Data;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{


    [Authorize]
    public class FractionOwnerController : Controller
    {
        private readonly IFractionOwnerRepository _fractionOwnerRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ConverterHelper _converterHelper;

        public FractionOwnerController(IFractionOwnerRepository fractionOwnerRepository, IUnitRepository unitRepository, ConverterHelper converterHelper)
        {
            _fractionOwnerRepository = fractionOwnerRepository;
            _unitRepository = unitRepository;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            var owners = await _fractionOwnerRepository.GetAllActiveAsync();
            var model = new FractionOwnerListViewModel
            {
                FractionOwners = _converterHelper.ToFractionOwnerViewModelList(owners).ToList(),
                TotalCount = owners.Count()
            };
            return View(model);
        }

        // Exibe o formulário para criar um novo proprietário de fração
        public async Task<IActionResult> Create()
        {
            var activeUnits = await _unitRepository.GetAllActiveAsync();
            ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName");
            return View(new FractionOwnerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FractionOwnerViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Dados inválidos.");
            }

            if (ModelState.IsValid)
            {
                var fractionOwner = _converterHelper.ToFractionOwner(model);
                await _fractionOwnerRepository.CreateWithRelationsAsync(fractionOwner);
                return RedirectToAction(nameof(Index));
            }
            var activeUnits = await _unitRepository.GetAllActiveAsync();
            ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName");
            return View(model);
        }

        // Exibe o formulário para editar um proprietário de fração
        public async Task<IActionResult> Edit(int id)
        {
            var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id);
            if (fractionOwner == null || fractionOwner.WasDeleted)
            {
                return NotFound();
            }
            var viewModel = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
            var activeUnits = await _unitRepository.GetAllActiveAsync();
            ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName", viewModel.UnitId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FractionOwnerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest("IDs inconsistentes.");
            }

            if (ModelState.IsValid)
            {
                var fractionOwner = _converterHelper.ToFractionOwner(viewModel); // Removido GetContext
                await _fractionOwnerRepository.UpdateWithRelationsAsync(fractionOwner); // Novo método
                return RedirectToAction(nameof(Index));
            }
            var activeUnits = await _unitRepository.GetAllActiveAsync();
            ViewBag.Units = new SelectList(activeUnits, "Id", "UnitName", viewModel.UnitId);
            return View(viewModel);
        }

        // Exibe a confirmação para deletar um proprietário de fração
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id);
            if (fractionOwner == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id);
            if (fractionOwner == null)
            {
                return NotFound();
            }

            fractionOwner.WasDeleted = true; // Exclusão lógica
            await _fractionOwnerRepository.UpdateAsync(fractionOwner);
            return RedirectToAction(nameof(Index));
        }
    }
}

