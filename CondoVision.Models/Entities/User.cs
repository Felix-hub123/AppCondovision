using CondoVision.Models.Entities;
using CondoVision.Models.Interface;
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
        [StringLength(255)]
        public string? FullName { get; set; }

      
        [Required]
        public string? UserType { get; set; }
               
        [Display(Name = "Image")]
        public Guid? ImageId { get; set; } 

       public bool WasDeleted { get; set; }


        public DateTime? LastLogin { get; set; }

        public string ProfileImageUrl { get; set; } = string.Empty;

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/users/noimage.png"
            : $"https://condovision.blob.core.windows.net/users/{ImageId}.jpg";

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }

        public string? TaxId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; }


        /// <summary>
        /// Navigation property for associated condominiums (if applicable, e.g., for managers).
        /// </summary>
        /// 
       
     

        public ICollection<CondominiumUser>? CondominiumUsers { get; set; }

        public ICollection<Company>? CreatedCompanies { get; set; }

        public ICollection<FractionOwner> FractionOwners { get; set; } = new List<FractionOwner>();

        public ICollection<RecentActivity>? RecentActivities { get; set; }

        public ICollection<Payment>? Payments { get; set; }


    }
}
