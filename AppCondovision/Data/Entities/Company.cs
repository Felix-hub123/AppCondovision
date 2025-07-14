using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? TaxNumber { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public bool WasDeleted { get; set; } = false;

        // Navigation properties

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Condominium> Condominiums { get; set; } = new List<Condominium>();

    }
}
