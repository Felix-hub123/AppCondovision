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
    public class FractionOwner : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UnitNumber { get; set; } = string.Empty; 

        [StringLength(50)]
        public string? FractionFloor { get; set; } 

        [StringLength(50)]
        public string? FractionBlock { get; set; } 

        [Required]
        public int UnitId { get; set; } 
        public Unit? Unit { get; set; } 

        [Required]
        public string UserId { get; set; } = string.Empty; 
        public User? User { get; set; } 

        [Required]
        [StringLength(100)]
        public string OwnerFullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string OwnerEmail { get; set; } = string.Empty;


        [Required] 
        public int? CompanyId { get; set; }


        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public bool WasDeleted { get; set; } = false;
    }
}
