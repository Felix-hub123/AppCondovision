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
    public class Payment : IEntity
    {

        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int CondominiumId { get; set; }

        [ForeignKey("CondominiumId")]
        public Condominium? Condominium { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public bool WasDeleted { get; set; }

       
    }
}
