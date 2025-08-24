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


    [Authorize(Roles = "CompanyAdmin,CondoManager")]
    public class FractionOwnerController : Controller
    {
        private readonly IFractionOwnerRepository _fractionOwnerRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;

        public FractionOwnerController(
            IFractionOwnerRepository fractionOwnerRepository,
            IUnitRepository unitRepository,
            IConverterHelper converterHelper,
            UserManager<User> userManager)
        {
            _fractionOwnerRepository = fractionOwnerRepository;
            _unitRepository = unitRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var fractionOwners = await _fractionOwnerRepository.GetAllActiveAsync();
            var viewModels = _converterHelper.ToFractionOwnerViewModelList(fractionOwners);
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id);
            if (fractionOwner == null || fractionOwner.WasDeleted)
            {
                return NotFound();
            }

            var viewModel = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
            return View(viewModel);
        }

        // Exibe o formulário para criar um novo proprietário de fração
        public async Task<IActionResult> Create()
        {
            var activeUnits = await _unitRepository.GetAllActiveAsync();
            if (activeUnits == null || !activeUnits.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhuma unidade ativa disponível. Verifique o seeding ou o banco de dados.");
            }
            ViewBag.Units = new SelectList(activeUnits ?? new List<Unit>(), "Id", "UnitName");

            var users = await _userManager.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum usuário disponível. Verifique o registro de usuários.");
            }
            ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");

            return View();
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
                try
                {
                    await _fractionOwnerRepository.CreateWithRelationsAsync(fractionOwner);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao criar proprietário: {ex.Message}");
                }
            }

            var activeUnits = await _unitRepository.GetAllActiveAsync();
            if (activeUnits == null || !activeUnits.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhuma unidade ativa disponível. Verifique o seeding ou o banco de dados.");
            }
            ViewBag.Units = new SelectList(activeUnits ?? new List<Unit>(), "Id", "UnitName");

            var users = await _userManager.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                ModelState.AddModelError(string.Empty, "Nenhum usuário disponível. Verifique o registro de usuários.");
            }
            ViewBag.Users = new SelectList(users ?? new List<User>(), "Id", "UserName");

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
                var fractionOwner = _converterHelper.ToFractionOwner(viewModel);
                await _fractionOwnerRepository.UpdateWithRelationsAsync(fractionOwner);
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

            fractionOwner.WasDeleted = true;
            await _fractionOwnerRepository.UpdateAsync(fractionOwner);
            return RedirectToAction(nameof(Index));
        }
    }
}

