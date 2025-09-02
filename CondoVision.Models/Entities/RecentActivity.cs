using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class RecentActivity : IEntity
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? Action { get; set; }

        public DateTime Date { get; set; }

        public User? User { get; set; }

        public bool WasDeleted { get; set; }


    }
}
