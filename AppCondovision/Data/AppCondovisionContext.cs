using AppCondovision.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AppCondovision.Data
{
    public class AppCondovisionContext : IdentityDbContext<User>
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Condominium> Condominiums { get; set; }
        public DbSet<Fraction> Fractions { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Quota> Quotas { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Intervention> Interventions { get; set; }
    

      public AppCondovisionContext(DbContextOptions<AppCondovisionContext> options)
           : base(options)
      {

      }
    }

}
