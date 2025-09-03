using CondoVision.Data.Entities;
using CondoVision.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models.Entities
{
    public class Occurrence : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }

        public int? FractionOwnerId { get; set; }

        [ForeignKey("FractionOwnerId")]
        public FractionOwner? FractionOwner { get; set; }

        [Required]
        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime OccurrenceDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } // Ex.: "Aberta", "Em Andamento", "Resolvida"

        public bool WasDeleted { get; set; }
    }
}
