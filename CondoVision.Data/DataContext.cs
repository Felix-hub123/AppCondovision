using CondoVision.Data.Entities;
using CondoVision.Models.Entities;
using CondoVision.Models.Interface;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
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

        public DbSet<Fraction> Fractions { get; set; }

        public DbSet<FractionOwner> FractionOwners { get; set; }

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

           
            builder.Entity<FractionOwner>()
               .HasOne(fo => fo.User)
               .WithMany(u => u.FractionOwners)
               .HasForeignKey(fo => fo.UserId)
               .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<FractionOwner>()
                .HasOne(fo => fo.Fraction)
                .WithMany(f => f.FractionOwners)
                .HasForeignKey(fo => fo.FractionId)
                .OnDelete(DeleteBehavior.Restrict);

           
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
               
                if (typeof(IEntity).IsAssignableFrom(entityType.ClrType) &&
                    entityType.FindProperty("WasDeleted") != null)
                {
                  
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                  
                    var property = Expression.Property(parameter, "WasDeleted");
                   
                    var filter = Expression.Lambda(Expression.Not(property), parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(filter);

                }
            }
        }




    }
  
}
