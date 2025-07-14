namespace AppCondovision.Data.Entities
{
    public class Quota
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }

        public int FractionId { get; set; }

        public decimal Value { get; set; }

        public DateTime DueDate { get; set; }

        public bool WasPaid { get; set; } = false;

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual Fraction Fraction { get; set; } = new Fraction();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
