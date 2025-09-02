using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(DataContext context) : base(context)
        {
        }
        public async Task<int> GetRecentActivitiesCountAsync()
        {
            return await _context.ActivityLogs
                .Where(a => a.Timestamp >= DateTime.Now.AddDays(-30) && !a.WasDeleted)
                .CountAsync();
        }

        public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int take = 5)
        {
            return await GetRecentAsync(take); // Reutiliza o método genérico
        }

        public async Task<List<(string Id, string UserName, string Action, DateTime Date)>> GetRecentActivitiesAsTupleAsync(int take = 5)
        {
            var activities = await GetRecentActivitiesAsync(take);
            return activities.Select(a => (a.Id.ToString(), a.UserName, a.Action, a.Timestamp)).ToList();
        }
    }
    

    
}
