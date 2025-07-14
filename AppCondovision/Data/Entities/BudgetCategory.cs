using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class BudgetCategory : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool WasDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}

