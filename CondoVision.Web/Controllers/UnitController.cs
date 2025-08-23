using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{
    public class UnitController : Controller
    {

        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConverterHelper _converterHelper;
        ICondominiumRepository _condominiumRepository;

        public UnitController(
            IUnitRepository unitRepository,
            IUserRepository userRepository,
            IConverterHelper converterHelper,
            ICondominiumRepository condominiumRepository)
        {
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _converterHelper = converterHelper;
            _condominiumRepository = condominiumRepository;
        }


        public async Task<IActionResult> Index(int? condominiumId)
        {
            var condominiums = await _condominiumRepository.GetAllAsync();
            ViewBag.Condominiums = new SelectList(condominiums, "Id", "Name");

            IEnumerable<UnitViewModel> units = Enumerable.Empty<UnitViewModel>();
            if (condominiumId.HasValue && condominiumId > 0)
            {
                var unitsData = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId.Value);
                units = _converterHelper.ToViewModel(unitsData);
                ViewBag.CondominiumId = condominiumId;
            }
            else if (!condominiumId.HasValue && ViewBag.CondominiumId != null)
            {
                
                condominiumId = (int)ViewBag.CondominiumId;
                if (condominiumId > 0)
                {
                    var unitsData = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId.Value);
                    units = _converterHelper.ToViewModel(unitsData);
                }
            }

            ViewBag.Units = units;
            return View();
        }

       

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID da unidade inválido.");
            }
               

            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
               

            var model = _converterHelper.ToViewModel(unit);
            return View(model);
        }

        public async Task<IActionResult> Create(int condominiumId)
        {
            if (condominiumId <= 0)
            {
                 return BadRequest("ID do condomínio inválido.");
            }
                
            ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(), "Id", "FullName");
            var model = new UnitViewModel { CondominiumId = condominiumId };
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitViewModel model)
        {
            if (model == null || model.CondominiumId <= 0)
            {
                return BadRequest("Dados inválidos.");
            }

            if (ModelState.IsValid)
            {
                var unit = _converterHelper.ToEntity(model);
                await _unitRepository.CreateAsync(unit);
                return RedirectToAction(nameof(Index), new { condominiumId = model.CondominiumId });
            }

            ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(), "Id", "FullName");
            return View(model);
        }

      

       
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID da unidade inválido.");
            }

            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
               

            ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(), "Id", "FullName");
            var model = _converterHelper.ToViewModel(unit);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UnitViewModel model)
        {
            if (id != model.Id || model.CondominiumId <= 0)
            {
                return BadRequest("IDs inconsistentes.");
            }
               

            if (ModelState.IsValid)
            {
                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null)
                {
                    return NotFound();
                }
                  

                _converterHelper.UpdateUnit(unit, model);
                await _unitRepository.UpdateAsync(unit);
                return RedirectToAction(nameof(Index), new { condominiumId = model.CondominiumId });
            }

            ViewBag.Owners = new SelectList(await _userRepository.GetAllAsync(), "Id", "FullName");
            return View(model);
        }

       
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID da unidade inválido.");
            }

            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
                return NotFound();

            var model = _converterHelper.ToViewModel(unit);
            return View(model);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID da unidade inválido.");
            }

            var unit = await _unitRepository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            try
            {
                await _unitRepository.DeleteAsync(unit);
                return RedirectToAction(nameof(Index), new { condominiumId = unit.CondominiumId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao eliminar unidade: {ex.Message}");
                return View(unit);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnitsByCondominiumId(int condominiumId)
        {
            if (condominiumId <= 0)
                return Json(new { success = false, message = "Condominium ID inválido." });

            var units = await _unitRepository.GetUnitsByCondominiumIdAsync(condominiumId);
            var unitModels = _converterHelper.ToViewModel(units);
            return Json(new { success = true, units = unitModels });
        }
    }
}
