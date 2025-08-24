using CondoVision.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class DashBoardViewModel
    {
        public int CompaniesCount { get; set; }
        public int UsersCount { get; set; }
        public int CondominiumsCount { get; set; }
        public int PendingInvoicesCount { get; set; }
        public List<int?> UnitCondominiumIds { get; set; } = new();
        public List<int?> UnitCounts { get; set; } = new();
        public List<ActivityViewModel> RecentActivities { get; set; } = new();

        public List<string>? FractionOwnerLabels { get; set; }

        public List<int>? FractionOwnerCounts { get; set; }

       
        public int FractionsCount { get; set; }
        public int FractionOwnersCount { get; set; }
    }
}
