using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class CondominiumUser : IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }
        public Condominium? Condominium { get; set; }

        public string? UserId { get; set; } 
        public User? User { get; set; }

        public bool WasDeleted { get; set; } = false;



    }


}
