using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class PrikhoperskoeDbContext : DbContext
    {
        private readonly string _connectionString = "Server=PC;Database=prikhoperskoe_po;Trusted_Connection=True;TrustServerCertificate=True;";

        public PrikhoperskoeDbContext()
        {
        }

        public PrikhoperskoeDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<District> Districts { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Brigade> Brigades { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }
        public DbSet<Worker> Workers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Brigade>()
                .HasOne(b => b.District)
                .WithMany(d => d.Brigades)
                .HasForeignKey(b => b.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.District)
                .WithMany(d => d.Incidents)
                .HasForeignKey(i => i.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.EquipmentType)
                .WithMany(e => e.Incidents)
                .HasForeignKey(i => i.EquipmentTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkLog>()
                .HasOne(w => w.Incident)
                .WithMany(i => i.WorkLogs)
                .HasForeignKey(w => w.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkLog>()
                .HasOne(w => w.Brigade)
                .WithMany(b => b.WorkLogs)
                .HasForeignKey(w => w.BrigadeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.Brigade)
                .WithMany(b => b.Workers)
                .HasForeignKey(w => w.BrigadeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}