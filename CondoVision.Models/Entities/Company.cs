using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Entities
{
    public class Company : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? CompanyTaxId { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? Contact { get; set; }

        [MaxLength(100)]
        public Guid? LogoId { get; set; }

        public DateTime CreationDate { get; set; }

        public bool WasDeleted { get; set; }

        public ICollection<User>? Users { get; set; }
        public ICollection<Condominium>? Condominiums { get; set; }
    }
}
