using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{

    public class CondoManagerController : Controller
    {
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IFractionOwnerRepository _fractionOwnerRepository;
        private readonly UserManager<User> _userManager;

        public CondoManagerController(
            ICondominiumRepository condominiumRepository,
            IFractionOwnerRepository fractionOwnerRepository,
            UserManager<User> userManager)
        {
            _condominiumRepository = condominiumRepository;
            _fractionOwnerRepository = fractionOwnerRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var companyId = user.CompanyId;
                var condominiums = await _condominiumRepository.GetAllActiveAsync(companyId);
                var fractionOwners = await _fractionOwnerRepository.GetAllActiveAsync(companyId);

                var model = new DashBoardViewModel
                {
                    CondominiumsCount = condominiums.Count(),
                    FractionsCount = fractionOwners.Count(), 
                    FractionOwnersCount = fractionOwners.Count()
                };
                ViewData["Title"] = "Painel de Gestor de Condomínios";
                ViewData["Message"] = "Bem-vindo ao Painel de Gestor de Condomínios!";
                return View(model);
            }
            catch (Exception ex)
            {
               
                var model = new DashBoardViewModel
                {
                    CondominiumsCount = 0,
                    FractionsCount = 0,
                    FractionOwnersCount = 0
                };
                ViewData["Title"] = "Painel de Gestor de Condomínios";
                ViewData["Message"] = "Erro ao carregar o painel.";
                return View(model);
            }
        }
    }
}
