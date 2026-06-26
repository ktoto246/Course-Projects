using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Data
{
    public class GasRegionDbContext : DbContext
    {
        public DbSet<Welder> Welders { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<WeldingMachine> WeldingMachines { get; set; }
        public DbSet<Joint> Joints { get; set; }
        public DbSet<Inspection> Inspections { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=PC;Database=GasRegionDb;Trusted_Connection=True;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Welder>()
                .Property(w => w.FullName)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Welder>()
                .Property(w => w.StampNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Pipeline>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<WeldingMachine>()
                .Property(w => w.InventoryNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<WeldingMachine>()
                .Property(w => w.Model)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Joint>()
                .Property(j => j.JointNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Joint>()
                .HasOne(j => j.Welder)
                .WithMany()
                .HasForeignKey(j => j.WelderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Joint>()
                .HasOne(j => j.Pipeline)
                .WithMany()
                .HasForeignKey(j => j.PipelineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Joint>()
                .HasOne(j => j.Machine)
                .WithMany()
                .HasForeignKey(j => j.MachineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .Property(i => i.InspectionMethod)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Joint)
                .WithMany()
                .HasForeignKey(i => i.JointId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
