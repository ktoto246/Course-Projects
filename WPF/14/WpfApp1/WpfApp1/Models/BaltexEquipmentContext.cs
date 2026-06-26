using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class BaltexEquipmentContext : DbContext
    {
        public DbSet<ProductionUnit> ProductionUnits { get; set; } = null!;
        public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
        public DbSet<EquipmentType> EquipmentTypes { get; set; } = null!;
        public DbSet<EquipmentStatus> EquipmentStatuses { get; set; } = null!;
        public DbSet<Equipment> Equipment { get; set; } = null!;
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=PC;Database=BaltexEquipmentDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees");
                e.HasKey(x => x.EmployeeID);
                e.Property(x => x.FullName).IsRequired().HasMaxLength(150);
                e.Property(x => x.Position).HasMaxLength(100);
            });

            modelBuilder.Entity<ProductionUnit>(e =>
            {
                e.ToTable("ProductionUnits");
                e.HasKey(x => x.UnitID);
                e.Property(x => x.UnitName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Manufacturer>(e =>
            {
                e.ToTable("Manufacturers");
                e.HasKey(x => x.ManufacturerID);
                e.Property(x => x.ManufacturerName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Country).HasMaxLength(50);
            });

            modelBuilder.Entity<EquipmentType>(e =>
            {
                e.ToTable("EquipmentTypes");
                e.HasKey(x => x.TypeID);
                e.Property(x => x.TypeName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<EquipmentStatus>(e =>
            {
                e.ToTable("EquipmentStatuses");
                e.HasKey(x => x.StatusID);
                e.Property(x => x.StatusName).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Equipment>(e =>
            {
                e.ToTable("Equipment");
                e.HasKey(x => x.EquipmentID);
                e.Property(x => x.InventoryNumber).IsRequired().HasMaxLength(20);
                e.HasIndex(x => x.InventoryNumber).IsUnique();
                e.Property(x => x.EquipmentName).IsRequired().HasMaxLength(150);
                e.Property(x => x.Model).HasMaxLength(100);
                e.Property(x => x.Notes).HasMaxLength(500);

                e.HasOne(x => x.Type).WithMany(t => t.Equipment).HasForeignKey(x => x.TypeID).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Manufacturer).WithMany(m => m.Equipment).HasForeignKey(x => x.ManufacturerID).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Unit).WithMany(u => u.Equipment).HasForeignKey(x => x.UnitID).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Status).WithMany(s => s.Equipment).HasForeignKey(x => x.StatusID).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MaintenanceRecord>(e =>
            {
                e.ToTable("MaintenanceRecords");
                e.HasKey(x => x.RecordID);
                e.Property(x => x.MaintenanceType).IsRequired().HasMaxLength(50);
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.Cost).HasPrecision(10, 2);
                e.HasOne(x => x.Equipment).WithMany(eq => eq.MaintenanceRecords).HasForeignKey(x => x.EquipmentID).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Employee).WithMany(emp => emp.MaintenanceRecords).HasForeignKey(x => x.EmployeeID).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}