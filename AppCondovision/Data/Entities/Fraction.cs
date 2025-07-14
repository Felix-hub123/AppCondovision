using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Fraction : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Number { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Floor { get; set; }

        public int CondominiumId { get; set; }
        public int? OwnerId { get; set; }

        public double? Area { get; set; }
        public double? Permillage { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual User? Owner { get; set; }
        public virtual ICollection<Quota> Quotas { get; set; } = new List<Quota>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}

