using CondoVision.Data;
using CondoVision.Models;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CondoVision.Web.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class CompanyAdminController : Controller
    {
        private readonly DataContext _context;

        public CompanyAdminController(DataContext context)
        {
            _context = context;
        }

        // GET: CompanyAdminController
        public async Task<ActionResult> Index()
        {
            try
            {
                var recentActivities = await _context.RecentActivities
                    .OrderByDescending(a => a.Date)
                    .Take(3)
                    .Select(a => new ActivityViewModel
                    {
                        Id = a.Id,
                        UserName = a.UserName!,  
                        Action = a.Action!,
                        Date = a.Date
                    })
                    .ToListAsync();

                var dashboardData = new DashBoardViewModel
                {
                    CompaniesCount = await _context.Companies.CountAsync(),
                    UsersCount = await _context.Users.CountAsync(),
                    CondominiumsCount = await _context.Condominiums.CountAsync(),
                    PendingInvoicesCount = 0,
                    UnitCondominiumIds = await _context.Units
                        .Where(u => !u.WasDeleted)
                        .GroupBy(u => u.CondominiumId)
                        .Select(g => (int?)g.Key)
                        .ToListAsync() ?? new List<int?>(),
                    UnitCounts = await _context.Units
                        .Where(u => !u.WasDeleted)
                        .GroupBy(u => u.CondominiumId)
                        .Select(g => (int?)g.Count())
                        .ToListAsync() ?? new List<int?>(),
                    RecentActivities = recentActivities
                };

                return View(dashboardData);
            }
            catch (Exception)
            {
                return View(new DashBoardViewModel());
            }
        }


    }
}
