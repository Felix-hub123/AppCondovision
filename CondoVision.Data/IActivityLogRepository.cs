using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public interface IActivityLogRepository: IGenericRepository<ActivityLog>
    {
        Task<int> GetRecentActivitiesCountAsync(); 

        Task<List<ActivityLog>> GetRecentActivitiesAsync(int take = 5); 

        Task<List<(string Id, string UserName, string Action, DateTime Date)>> GetRecentActivitiesAsTupleAsync(int take = 5);
    }
}
