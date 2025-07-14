using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<User>? Users { get; set; } = new List<User>();

        public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
        public virtual ICollection<Condominium> Condominiums { get; set; } = new List<Condominium>();

    }
}
