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
        public DbSet<Payment> Payments { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<ForumPost> ForumPosts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definição das chaves primárias
            modelBuilder.Entity<Company>().HasKey(c => c.Id);
            modelBuilder.Entity<Condominium>().HasKey(c => c.Id);
            modelBuilder.Entity<CondominiumUser>().HasKey(cu => cu.Id);
            modelBuilder.Entity<Unit>().HasKey(u => u.Id);
            modelBuilder.Entity<FractionOwner>().HasKey(fo => fo.Id);
            modelBuilder.Entity<RecentActivity>().HasKey(ra => ra.Id);
            modelBuilder.Entity<Payment>().HasKey(p => p.Id);

            // Relacionamentos existentes
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

            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Condominium)
                .WithMany(c => c.Units)
                .HasForeignKey(u => u.CondominiumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Units)
                .HasForeignKey(u => u.CompanyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Condominium>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Condominiums)
                .HasForeignKey(c => c.CompanyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true)
                .HasConstraintName("FK_Company_CreatedBy_UserId");

            modelBuilder.Entity<FractionOwner>()
                .HasOne(fo => fo.Unit)
                .WithMany(u => u.FractionOwners)
                .HasForeignKey(fo => fo.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FractionOwner>()
                .HasOne(fo => fo.User)
                .WithMany(u => u.FractionOwners)
                .HasForeignKey(fo => fo.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecentActivity>()
                .HasOne(ra => ra.User)
                .WithMany(u => u.RecentActivities)
                .HasForeignKey(ra => ra.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .Property(e => e.CreatedById)
                .HasColumnType("nvarchar(450)")
                .IsRequired(true);


            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Condominium)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CondominiumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10, 2)");

            // Configuração de soft delete para todas as entidades
            modelBuilder.Entity<Company>().Property(c => c.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Condominium>().Property(c => c.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<CondominiumUser>().Property(cu => cu.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Unit>().Property(u => u.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<FractionOwner>().Property(fo => fo.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<RecentActivity>().Property(ra => ra.WasDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Payment>().Property(p => p.WasDeleted).HasDefaultValue(false);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.WasDeleted).HasDefaultValue(false);
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}





