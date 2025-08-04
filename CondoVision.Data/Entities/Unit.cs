using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data.Entities
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? UnitName { get; set; }

        [Required]
        public decimal OwnershipShare { get; set; }

        [Required]
        public int CondominiumId { get; set; }
        [ForeignKey("CondominiumId")]
        public Condominium? Condominium { get; set; }

        // Current Owner
        public string? OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }
    }
}
