using CondoVision.Data;
using CondoVision.Data.Helper;
using CondoVision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoVision.Web.Controllers
{
    public class FractionOwnerController : Controller
    {
        private readonly IFractionOwnerRepository _fractionOwnerRepository;
        private readonly IFractionRepository _fractionRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public FractionOwnerController(
            IFractionOwnerRepository fractionOwnerRepository,
            IFractionRepository fractionRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper)
        {
            _fractionOwnerRepository = fractionOwnerRepository;
            _fractionRepository = fractionRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }




        public async Task<IActionResult> Index()
        {
            var fractionOwners = await _fractionOwnerRepository.GetAllFractionOwnersWithDetailsAsync();
            var model = fractionOwners.Select(fo => _converterHelper.ToFractionOwnerViewModel(fo)).ToList();
            return View(model);
        }


        // GET: FractionOwner/Associate
        public async Task<IActionResult> Associate()
        {
            var model = new AssociateFractionOwnerViewModel
            {
                Fractions = await GetFractionsSelectList(),
                Users = await GetCondoOwnersSelectList()
            };
            return View(model);
        }


        // POST: FractionOwner/Associate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(AssociateFractionOwnerViewModel model)
        {
            if (await _fractionOwnerRepository.FractionOwnerExistsAsync(model.FractionId, model.UserId ?? ""))
            {
                ModelState.AddModelError(string.Empty, "Esta fração já está associada a este utilizador.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var fractionOwner = _converterHelper.ToFractionOwner(model);
                    await _fractionOwnerRepository.AddAsync(fractionOwner);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Erro ao associar proprietário: {ex.Message}");
                }
            }


            model.Fractions = await GetFractionsSelectList();
            model.Users = await GetCondoOwnersSelectList();
            return View(model);
        }


        // GET: FractionOwner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fractionOwner = await _fractionOwnerRepository.GetFractionOwnerWithDetailsAsync(id.Value);
            if (fractionOwner == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
            return View(model);
        }


        // GET: FractionOwner/Disassociate/5
        public async Task<IActionResult> Disassociate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fractionOwner = await _fractionOwnerRepository.GetFractionOwnerWithDetailsAsync(id.Value);
            if (fractionOwner == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
            return View(model);
        }

        // POST: FractionOwner/Disassociate/5
        [HttpPost, ActionName("Disassociate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisassociateConfirmed(int id)
        {
            var fractionOwner = await _fractionOwnerRepository.GetByIdAsync(id);

            if (fractionOwner == null)
            {
                return NotFound();
            }

            try
            {
                fractionOwner.WasDeleted = true; // Soft delete
                await _fractionOwnerRepository.UpdateAsync(fractionOwner);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao desassociar: {ex.Message}");
                var model = _converterHelper.ToFractionOwnerViewModel(fractionOwner);
                return View("Disassociate", model); // Retorna à vista de confirmação com erro
            }
        }



        // MÉTODOS AUXILIARES PARA POPULAR DROPDOWNS
        // Obtém SelectListItems de todas as frações ativas
        private async Task<IEnumerable<SelectListItem>> GetFractionsSelectList()
        {
            var fractions = await _fractionRepository.GetAllAsync(); 
            return fractions.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = $"{f.UnitNumber} ({f.Condominium?.Name ?? "N/A"})" 
            }).OrderBy(f => f.Text);
        }

        // Obtém SelectListItems de utilizadores com a role 'CondoOwner'
        private async Task<IEnumerable<SelectListItem>> GetCondoOwnersSelectList()
        {
           
            var users = await _userHelper.GetUsersInRoleAsync("CondoOwner"); 
            return users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.FullName} ({u.Email})"
            }).OrderBy(u => u.Text);
        }
    }
}
