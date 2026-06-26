using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<MovementHistory> MovementHistories { get; set; }
        public DbSet<RepairHistory> RepairHistories { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=EquipmentAccountingDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Category).WithMany(c => c.Equipments)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Status).WithMany(s => s.Equipments)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Employee).WithMany(emp => emp.Equipments)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Cabinet).WithMany(cab => cab.Equipments)
                .HasForeignKey(e => e.CabinetId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<MovementHistory>()
                .HasOne(m => m.FromEmployee).WithMany()
                .HasForeignKey(m => m.FromEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovementHistory>()
                .HasOne(m => m.ToEmployee).WithMany()
                .HasForeignKey(m => m.ToEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}