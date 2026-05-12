using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.Employee> Employees { get; set; }
        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Crop> Crops { get; set; }
        public DbSet<Models.Storage> Storages { get; set; }
        public DbSet<Models.Service> Services { get; set; }
        public DbSet<Models.GrainBatch> GrainBatches { get; set; }
        public DbSet<Models.LabTest> LabTests { get; set; }
        public DbSet<Models.RenderedService> RenderedServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=BalashovBreadBaseDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.LabTest>()
                .HasOne(l => l.Batch)
                .WithOne(b => b.LabTest)
                .HasForeignKey<Models.LabTest>(l => l.BatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.GrainBatch>()
                .HasOne(b => b.Weigher)
                .WithMany(e => e.WeighedBatches)
                .HasForeignKey(b => b.WeigherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.LabTest>()
                .HasOne(l => l.LabTech)
                .WithMany(e => e.LabTests)
                .HasForeignKey(l => l.LabTechId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.RenderedService>()
                .HasOne(r => r.Manager)
                .WithMany(e => e.ProcessedServices)
                .HasForeignKey(r => r.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.GrainBatch>()
                .HasOne(b => b.Client)
                .WithMany(c => c.Batches)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.GrainBatch>()
                .HasOne(b => b.Crop)
                .WithMany(c => c.Batches)
                .HasForeignKey(b => b.CropId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}