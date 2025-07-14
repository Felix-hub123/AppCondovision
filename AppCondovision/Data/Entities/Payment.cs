namespace AppCondovision.Data.Entities
{
    public class Payment : IEntity
    {
        public int Id { get; set; }

        public int QuotaId { get; set; }
        public int? FractionId { get; set; }
        public int? UserId { get; set; }

        public decimal Value { get; set; }
        public DateTime PaymentDate { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Quota Quota { get; set; } = new Quota();
        public virtual Fraction? Fraction { get; set; }
        public virtual User? User { get; set; }
    }
}
