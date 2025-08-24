using CondoVision.Data;
using CondoVision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly DataContext _context;

        public DashBoardController(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            var companiesCountTask = _context.Companies.CountAsync();
            var usersCountTask = _context.Users.CountAsync();
            var condominiumsCountTask = _context.Condominiums.CountAsync();

            // Agrupar unidades por condomínio
            var unitGroupsTask = _context.Units
                .GroupBy(u => u.CondominiumId)
                .Select(g => new { CondominiumId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Obter últimas 10 atividades
            var recentActivitiesTask = _context.RecentActivities
                .OrderByDescending(a => a.Date)
                .Take(10)
                .ToListAsync();

            await Task.WhenAll(companiesCountTask, usersCountTask, condominiumsCountTask, unitGroupsTask, recentActivitiesTask);

            var unitGroups = unitGroupsTask.Result;
            var recentActivities = recentActivitiesTask.Result;

            // Montar a ViewModel do Dashboard
            var dashboardVM = new DashBoardViewModel
            {
                CompaniesCount = companiesCountTask.Result,
                UsersCount = usersCountTask.Result,
                CondominiumsCount = condominiumsCountTask.Result,
                PendingInvoicesCount = 65, // Placeholder

                UnitCondominiumIds = unitGroups.Select(g => (int?)g.CondominiumId).ToList(),
                UnitCounts = unitGroups.Select(g => (int?)g.Count).ToList(),

                RecentActivities = recentActivities.Select(a => new ActivityViewModel
                {
                    Id = a.Id,
                    UserName = a.UserName ?? "Desconhecido",
                    Action = a.Action ?? string.Empty,
                    Date = a.Date
                }).ToList()
            };

            ViewData["Title"] = "Painel do Administrador";

            return View(dashboardVM);
        }
    }
    
}
