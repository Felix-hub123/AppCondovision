using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class RecentActivity
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public DateTime Date { get; set; }
       
    }
}
