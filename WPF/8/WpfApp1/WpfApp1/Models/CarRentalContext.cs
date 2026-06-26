using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Models
{
    public partial class CarRentalContext : DbContext
    {
        public CarRentalContext() { }
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public virtual DbSet<Автомобиль> Автомобили { get; set; }
        public virtual DbSet<Клиент> Клиенты { get; set; }
        public virtual DbSet<Аренда> Аренды { get; set; }
        public virtual DbSet<ДТП> ДТП_Записи { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=ПрокатАвтомобилей;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}