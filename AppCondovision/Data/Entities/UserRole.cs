using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Principal;

namespace AppCondovision.Data.Entities
{
    public class UserRole : IdentityUserRole<int>, IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public new int UserId { get; set; }


        [Required]
        public new int RoleId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual User? User { get; set; } 
        public virtual Role? Role { get; set; } 

    }
}
