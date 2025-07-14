using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Meeting : IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime MeetingDate { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual ICollection<User> Participants { get; set; } = new List<User>();
    }
}

