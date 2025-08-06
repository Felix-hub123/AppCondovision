using CondoVision.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace CondoVision.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Condominium> Condominiums { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           
            builder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId);

           
            builder.Entity<Unit>()
                .HasOne(u => u.Owner)
                .WithMany(o => o.OwnedUnits)
                .HasForeignKey(u => u.OwnerId);

           
            builder.Entity<Unit>()
                .Property(u => u.OwnershipShare)
                .HasPrecision(18, 4);

        }




    }
  
}
