using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Expense : IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }
        public int? BudgetId { get; set; }
        public int? BudgetCategoryId { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        public decimal Value { get; set; }
        public DateTime ExpenseDate { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual Condominium Condominium { get; set; } = new Condominium();
        public virtual Budget? Budget { get; set; }
        public virtual BudgetCategory? BudgetCategory { get; set; }
    }
}
