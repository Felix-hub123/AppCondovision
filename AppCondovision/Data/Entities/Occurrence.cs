using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Occurrence : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public int CondominiumId { get; set; }
        public int? UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual User? User { get; set; }
    }
}

