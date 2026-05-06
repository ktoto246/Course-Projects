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

        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=EquipmentAccountingDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}