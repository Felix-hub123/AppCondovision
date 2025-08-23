using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CondoVision.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserHelper _userHelper;
    private readonly UserManager<User> _userManager;

    public HomeController(
        ILogger<HomeController> logger,
        IUserHelper userHelper,
        UserManager<User> userManager)
    {
        _logger = logger;
        _userHelper = userHelper;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            _logger.LogInformation("Utilizador autenticado, redirecionando para Painel.");
            ViewData["UserManager"] = _userManager; 
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
            _logger.LogWarning("Tentativa de acesso ao Painel sem autenticação, redirecionando para Index.");
            return RedirectToAction("Index", "Home");
        }

        ViewData["UserManager"] = _userManager; 

        if (User.IsInRole("CompanyAdmin"))
        {
            _logger.LogInformation("Utilizador CompanyAdmin redirecionado para CompanyAdmin/Index.");
            return RedirectToAction("Index", "CompanyAdmin");
        }
        else if (User.IsInRole("CondoManager"))
        {
            _logger.LogInformation("Utilizador CondoManager redirecionado para CondoManager/Index.");
            return RedirectToAction("Index", "CondoManager");
        }
        else if (User.IsInRole("CondoOwner"))
        {
            _logger.LogInformation("Utilizador CondoOwner redirecionado para CondoOwner/Index.");
            return RedirectToAction("Index", "CondoOwner");
        }

       
        _logger.LogWarning("Utilizador autenticado sem role específica, redirecionando para DashboardGenérico.");
        return RedirectToAction("GenericDashBoard", "Home");
    }

    public IActionResult GenericDashBoard()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        _logger.LogInformation("Exibindo DashboardGenérico para utilizador autenticado sem role específica.");
        ViewData["UserManager"] = _userManager; // Passar UserManager para o layout
        return View();
    }


}
