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
    public class Notification : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CondominiumId { get; set; }

        [ForeignKey("CondominiumId")]
        public Condominium? Condominium { get; set; }

        [Required]
        [StringLength(500)]
        public string? Message { get; set; }

        public DateTime NotificationDate { get; set; }

        public bool IsRead { get; set; }

        public bool WasDeleted { get; set; }
    }
}
