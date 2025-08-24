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
using static CondoVision.Data.Entities.User;


namespace CondoVision.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Condominium> Condominiums { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<CondominiumUser> CondominiumUsers { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<RecentActivity> RecentActivities { get; set; }

        public DbSet<FractionOwner> FractionOwners { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasKey(c => c.Id);
            modelBuilder.Entity<Condominium>().HasKey(c => c.Id);
            modelBuilder.Entity<CondominiumUser>().HasKey(cu => cu.Id); 
            modelBuilder.Entity<Unit>().HasKey(u => u.Id);

            // Configuração de CondominiumUser
            modelBuilder.Entity<CondominiumUser>()
                .HasOne(cu => cu.Condominium)
                .WithMany(c => c.CondominiumUsers)
                .HasForeignKey(cu => cu.CondominiumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CondominiumUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CondominiumUsers) 
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Configuração de Unit
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Condominium)
                .WithMany(c => c.Units)
                .HasForeignKey(u => u.CondominiumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Owner)
                .WithMany(u => u.Units)
                .HasForeignKey(u => u.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de Condominium
            modelBuilder.Entity<Condominium>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Condominiums)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração de valores padrão
            modelBuilder.Entity<Company>().Property(c => c.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Condominium>().Property(c => c.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<CondominiumUser>().Property(cu => cu.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Unit>().Property(u => u.WasDeleted).HasDefaultValue(false);
        }
    }
}





