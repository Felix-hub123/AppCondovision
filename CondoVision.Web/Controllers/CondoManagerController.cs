using CondoVision.Data;
using CondoVision.Models;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{

    public class CondoManagerController : Controller
    {
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IFractionOwnerRepository _fractionOwnerRepository;

        public CondoManagerController(
            ICondominiumRepository condominiumRepository,
            IFractionOwnerRepository fractionOwnerRepository)
        {
            _condominiumRepository = condominiumRepository;
            _fractionOwnerRepository = fractionOwnerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var condominiums = await _condominiumRepository.GetAllActiveAsync();
            var fractionOwners = await _fractionOwnerRepository.GetAllActiveAsync();

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
    }
}
