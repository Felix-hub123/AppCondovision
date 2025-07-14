using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Budget : IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }
        public int? BudgetCategoryId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal? TotalValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual BudgetCategory? BudgetCategory { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
