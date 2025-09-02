using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class ActivityLog : IEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; }
        public bool WasDeleted { get; set; }
    }
}
