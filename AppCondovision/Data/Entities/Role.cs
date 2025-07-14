using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Role : IdentityRole<int>, IEntity
    {
     

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();


    }
}
