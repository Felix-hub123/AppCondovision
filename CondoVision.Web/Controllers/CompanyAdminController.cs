using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CondoVision.Web.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyAdminController : Controller
    {
       
        // GET: CompanyAdminController
        public ActionResult Index()
        {
            ViewData["Title"] = "Painel de Administrador da Empresa";
            ViewData["Message"] = "Bem-vindo ao Painel de Administrador da Empresa!";
            return View();
        }

       
    }
}
