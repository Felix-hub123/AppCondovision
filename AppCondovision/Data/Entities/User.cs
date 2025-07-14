using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class User : IdentityUser<int>, IEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Address { get; set; }

        public int? CityId { get; set; }
        public int? CompanyId { get; set; }

        [MaxLength(20)]
        public string? DocumentNumber { get; set; }

        [MaxLength(20)]
        public string? DocumentType { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool WasDeleted { get; set; } = false;

     
        public virtual City? City { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    
        public string FullName => $"{FirstName} {LastName}";
    }
}
