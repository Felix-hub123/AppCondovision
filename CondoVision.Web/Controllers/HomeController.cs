using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models;
using CondoVision.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CondoVision.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserHelper _userHelper;
    private readonly UserManager<User> _userManager;
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IForumRepository _forumRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICondominiumRepository _condominiumRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IUserHelper userHelper,
        UserManager<User> userManager,
        IActivityLogRepository activityLogRepository,
        IForumRepository forumRepository,
        ICompanyRepository companyRepository,
        ICondominiumRepository condominiumRepository)
    {
        _logger = logger;
        _userHelper = userHelper;
        _userManager = userManager;
        _activityLogRepository = activityLogRepository;
        _forumRepository = forumRepository;
        _companyRepository = companyRepository;
        _condominiumRepository = condominiumRepository;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            _logger.LogInformation("Utilizador autenticado, redirecionando para Painel. [Time: 05:40 PM WEST, 31-Aug-2025]");
            ViewData["UserManager"] = _userManager;
            return RedirectToAction("Painel", "Home");
        }

        _logger.LogInformation("Utilizador não autenticado, mostrando a página inicial. [Time: 05:40 PM WEST, 31-Aug-2025]");
        var model = new HomeViewModel
        {
            TotalCompanies = await _companyRepository.GetAllAsync().ContinueWith(t => t.Result.Count()),
            TotalCondominiums = await _condominiumRepository.GetAllAsync().ContinueWith(t => t.Result.Count()),
            WelcomeMessage = "Bem-vindo ao Sistema de Gestão de Condomínios! Registe-se ou faça login para gerir a sua empresa ou condomínio.",
            RegistrationEnabled = true,
            LoginEnabled = true
        };

        return View(model);
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
    public async Task<IActionResult> Painel()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            _logger.LogWarning("Tentativa de acesso ao Painel sem autenticação, redirecionando para Index. [Time: 05:40 PM WEST, 31-Aug-2025]");
            return RedirectToAction("Index", "Home");
        }

        ViewData["UserManager"] = _userManager;

        if (User.IsInRole("CompanyAdmin"))
        {
            _logger.LogInformation("Utilizador CompanyAdmin redirecionado para CompanyAdmin/Index. [Time: 05:40 PM WEST, 31-Aug-2025]");
            return RedirectToAction("Index", "CompanyAdmin");
        }
        else if (User.IsInRole("CondoManager"))
        {
            _logger.LogInformation("Utilizador CondoManager redirecionado para CondoManager/Index. [Time: 05:40 PM WEST, 31-Aug-2025]");
            return RedirectToAction("Index", "CondoManager");
        }
        else if (User.IsInRole("CondoOwner"))
        {
            _logger.LogInformation("Utilizador CondoOwner redirecionado para CondoOwner/Index. [Time: 05:40 PM WEST, 31-Aug-2025]");
            return RedirectToAction("Index", "CondoOwner");
        }

        _logger.LogWarning("Utilizador autenticado sem role específica, redirecionando para DashboardGenérico. [Time: 05:40 PM WEST, 31-Aug-2025]");
        return RedirectToAction("GenericDashBoard", "Home");
    }

    [Authorize]
    public async Task<IActionResult> GenericDashBoard()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        _logger.LogInformation("Exibindo DashboardGenérico para utilizador autenticado sem role específica. [Time: 05:40 PM WEST, 31-Aug-2025]");
        ViewData["UserManager"] = _userManager;

        var model = new HomeViewModel
        {
            TotalCompanies = await _companyRepository.GetAllAsync().ContinueWith(t => t.Result.Count()),
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalCondominiums = await _condominiumRepository.GetAllAsync().ContinueWith(t => t.Result.Count()),
            TotalActivities = await _activityLogRepository.GetAllAsync().ContinueWith(t => t.Result.Count()),
            NewUsersCount = await _userManager.Users.CountAsync(u => u.CreatedDate >= DateTime.Now.AddDays(-30)), // Filtro de 30 dias
            ActivityMonths = new List<string> { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun" },
            ActivityCounts = new List<int> { 10, 15, 20, 25, 30, 35 },
            CalendarEvents = new List<(string, DateTime, DateTime?, string, string, bool)>
                {
                    ("Assembleia Geral", new DateTime(2025, 9, 15), null, "#f39c12", "#f39c12", true),
                    ("Manutenção Elevador", new DateTime(2025, 9, 18, 10, 0, 0), new DateTime(2025, 9, 18, 12, 0, 0), "#00c0ef", "#00c0ef", false)
                },
            CondominiumLocations = new List<(string, double, double)>
                {
                    ("Condo Alfa", 38.7167, -9.1333),
                    ("Condo Beta", 38.7253, -9.1500)
                },
            WeatherTemperature = 22,
            WeatherLocation = "Lisboa, PT",
            WeatherCondition = "Ensolarado",
            RecentUsers = (await _userManager.Users.OrderByDescending(u => u.LockoutEnd ?? DateTimeOffset.Now).Take(3).ToListAsync())
                .Select(u => (FullName: u.UserName, ProfileImageUrl: u.ProfileImageUrl ?? "/dist/img/user2-160x160.jpg", LastActivity: u.LockoutEnd?.DateTime)).ToList(),
            RecentNews = new List<(string, DateTime)>
                {
                    ("Nova funcionalidade de chat", new DateTime(2025, 8, 31)),
                    ("Reunião anual marcada", new DateTime(2025, 8, 30))
                },
            ForumPosts = await _forumRepository.GetRecentPostsAsTupleAsync(2)
        };

        return View(model);
    }
}



