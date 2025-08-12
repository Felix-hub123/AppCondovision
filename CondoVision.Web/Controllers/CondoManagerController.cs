using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{

    public class CondoManagerController : Controller
    {

        // GET: CondoManagerController  
        public IActionResult Index()
        {
            ViewData["Title"] = "Painel de Gestor de Condomínios";
            ViewData["Message"] = "Bem-vindo ao Painel de Gestor de Condomínios!";
            return View();
        }
    }
}
