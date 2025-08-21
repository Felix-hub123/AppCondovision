using CondoVision.Models.Entities;
using CondoVision.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CondoVision.Data.Entities
{
    public class Condominium : IEntity
    {


        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool WasDeleted { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();

             
        public ICollection<CondominiumUser>? CondominiumUsers { get; set; }
      
    }

}
