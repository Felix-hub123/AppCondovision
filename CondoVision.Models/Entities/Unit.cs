using CondoVision.Models.Entities;
using CondoVision.Models.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondoVision.Data.Entities
{
    public class Unit : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? UnitName { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Permillage { get; set; }


        public int? CompanyId { get; set; }


        public Company? Company { get; set; }

        [Required]
        public int CondominiumId { get; set; }

        public string? OwnerId { get; set; }

        public Condominium? Condominium { get; set; }


        public ICollection<FractionOwner>? FractionOwners { get; set; }


        public bool WasDeleted { get; set; }
    }
}



