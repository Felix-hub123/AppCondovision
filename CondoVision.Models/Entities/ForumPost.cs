using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class ForumPost  : IEntity
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }

        public bool WasDeleted { get; set; }
    }
}
