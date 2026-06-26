using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ElanCheeseApp.Models
{
    public class CheeseVariety
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int MaturationDays { get; set; }
    }

    public class StorageChamber
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ChamberNumber { get; set; }
        [Required]
        public decimal Temperature { get; set; }
        [Required]
        public decimal Humidity { get; set; }
    }

    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;
    }

    public class CheeseBatch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VarietyId { get; set; }
        [Required]
        public int ChamberId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public decimal WeightKg { get; set; }

        [ForeignKey(nameof(VarietyId))]
        public CheeseVariety? Variety { get; set; }
        [ForeignKey(nameof(ChamberId))]
        public StorageChamber? Chamber { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }
    }

    public class QualityInspection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BatchId { get; set; }
        [Required]
        public DateTime InspectionDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey(nameof(BatchId))]
        public CheeseBatch? Batch { get; set; }
    }

    public class ElanCheeseDbContext : DbContext
    {
        public DbSet<CheeseVariety> CheeseVarieties { get; set; }
        public DbSet<StorageChamber> StorageChambers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CheeseBatch> CheeseBatches { get; set; }
        public DbSet<QualityInspection> QualityInspections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=PC;Database=ElanCheeseDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheeseBatch>()
                .HasOne(b => b.Variety)
                .WithMany()
                .HasForeignKey(b => b.VarietyId);

            modelBuilder.Entity<CheeseBatch>()
                .HasOne(b => b.Chamber)
                .WithMany()
                .HasForeignKey(b => b.ChamberId);

            modelBuilder.Entity<CheeseBatch>()
                .HasOne(b => b.Employee)
                .WithMany()
                .HasForeignKey(b => b.EmployeeId);

            modelBuilder.Entity<QualityInspection>()
                .HasOne(q => q.Batch)
                .WithMany()
                .HasForeignKey(q => q.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
