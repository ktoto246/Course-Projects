using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WpfApp1
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public List<GrainBatch> GrainBatches { get; set; } = new();
    }

    public class LaboratoryStaff
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;

        public List<AnalysisLog> AnalysisLogs { get; set; } = new();
    }

    public class GrainBatch
    {
        public int BatchId { get; set; }
        public int SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string VehicleNumber { get; set; } = null!;
        public decimal WeightTons { get; set; }

        public Supplier Supplier { get; set; } = null!;
        public List<AnalysisLog> AnalysisLogs { get; set; } = new();
    }

    public class QualityIndicator
    {
        public int IndicatorId { get; set; }
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal MinNormalValue { get; set; }
        public decimal MaxNormalValue { get; set; }

        public List<AnalysisLog> AnalysisLogs { get; set; } = new();
    }

    public class AnalysisLog
    {
        public int AnalysisId { get; set; }
        public int BatchId { get; set; }
        public int StaffId { get; set; }
        public int IndicatorId { get; set; }
        public decimal AnalysisValue { get; set; }
        public DateTime AnalysisDate { get; set; }

        public GrainBatch GrainBatch { get; set; } = null!;
        public LaboratoryStaff LaboratoryStaff { get; set; } = null!;
        public QualityIndicator QualityIndicator { get; set; } = null!;
    }

    public class BalashovDbContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<LaboratoryStaff> LaboratoryStaff { get; set; } = null!;
        public DbSet<GrainBatch> GrainBatches { get; set; } = null!;
        public DbSet<QualityIndicator> QualityIndicators { get; set; } = null!;
        public DbSet<AnalysisLog> AnalysisLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PC;Database=BalashovGrainQualityDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().ToTable("Suppliers");
            modelBuilder.Entity<LaboratoryStaff>().ToTable("LaboratoryStaff");
            modelBuilder.Entity<GrainBatch>().ToTable("GrainBatches");
            modelBuilder.Entity<QualityIndicator>().ToTable("QualityIndicators");
            modelBuilder.Entity<AnalysisLog>().ToTable("AnalysisLog");

            modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);
            modelBuilder.Entity<LaboratoryStaff>().HasKey(l => l.StaffId);
            modelBuilder.Entity<GrainBatch>().HasKey(g => g.BatchId);
            modelBuilder.Entity<QualityIndicator>().HasKey(q => q.IndicatorId);
            modelBuilder.Entity<AnalysisLog>().HasKey(a => a.AnalysisId);

            modelBuilder.Entity<GrainBatch>()
                .HasOne(g => g.Supplier)
                .WithMany(s => s.GrainBatches)
                .HasForeignKey(g => g.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnalysisLog>()
                .HasOne(a => a.GrainBatch)
                .WithMany(g => g.AnalysisLogs)
                .HasForeignKey(a => a.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnalysisLog>()
                .HasOne(a => a.LaboratoryStaff)
                .WithMany(l => l.AnalysisLogs)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnalysisLog>()
                .HasOne(a => a.QualityIndicator)
                .WithMany(q => q.AnalysisLogs)
                .HasForeignKey(a => a.IndicatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}