using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{
    [Authorize(Roles = "CondoOwner")]
    public class CondoOwnerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Painel de Proprietário de Condomínio";
            ViewData["Message"] = "Bem-vindo ao seu Painel de Proprietário!";
            return View();
        }
    }
}
