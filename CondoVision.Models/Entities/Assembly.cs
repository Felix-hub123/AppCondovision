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
    public class Assembly : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        public DateTime AssemblyDate { get; set; }

        [Required]
        public int CondominiumId { get; set; }

        [ForeignKey("CondominiumId")]
        public Condominium? Condominium { get; set; }

        [Required]
        [StringLength(1000)]
        public string? Minutes { get; set; }

        public bool IsPublished { get; set; }

        public bool WasDeleted { get; set; }
    }
}
