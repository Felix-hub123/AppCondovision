using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondoVision.Data.Entities
{
    public enum UserType
    {
        CompanyAdministrator,
        CondominiumManager,
        Resident,
        Employee
    }

    public class User : IdentityUser 
    {
        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? TaxId { get; set; }

       
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

       
        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }

        public UserType UserType { get; set; }

        public DateTime DateOfBirth { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }

       [Display(Name = "Image")]
        public Guid? ImageId { get; set; } 

       public bool WasDeleted { get; set; }

        public ICollection<FractionOwner>? FractionOwners { get; set; }

        public ICollection<Unit>? OwnedUnits { get; set; }
        public ICollection<Condominium>? ManagedCondominiums { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/users/noimage.png"
            : $"https://condovision.blob.core.windows.net/users/{ImageId}.jpg";
    }
}
