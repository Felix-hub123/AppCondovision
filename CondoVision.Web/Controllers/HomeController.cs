using CondoVision.Data.Helper;
using CondoVision.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CondoVision.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserHelper _userHelper;

    public HomeController(
        ILogger<HomeController> logger,
        IUserHelper userHelper)
    {
        _logger = logger;
        _userHelper = userHelper;
    }

    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            _logger.LogInformation("Utilizador autenticado, redirecionando para Painel.");
          
            return RedirectToAction("Painel", "Home");
        }
        _logger.LogInformation("Utilizador não autenticado, mostrando a página inicial.");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize] 
    public IActionResult Painel()
    {
        
        if (!User.Identity!.IsAuthenticated)
        {
          
            return RedirectToAction("Index", "Home");
        }

      
        if (User.IsInRole("CompanyAdmin"))
        {
           
            return RedirectToAction("Index", "CompanyAdmin"); 
        }
        else if (User.IsInRole("CondoManager"))
        {
           
            return RedirectToAction("Index", "CondoManager"); 
        }
        else if (User.IsInRole("CondoOwner"))
        {
            
            return RedirectToAction("Index", "CondoOwner"); // Assumindo CondoOwnerController
        }
       

        // 3. Redirecionamento padrão para utilizadores autenticados sem uma role específica
        // Se o utilizador está autenticado mas não tem nenhuma das roles acima,
        // ou tem uma role não coberta, pode redirecionar para uma página geral de dashboard
        // ou de volta para a Index (que pode ser um dashboard simples para todos os logados)
        return RedirectToAction("Index", "Home");
    }


}
