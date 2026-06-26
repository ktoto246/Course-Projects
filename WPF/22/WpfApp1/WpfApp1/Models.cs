using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Client
    {
        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("company_name")]
        public string? CompanyName { get; set; }

        [Column("contact_name")]
        public string ContactName { get; set; } = null!;

        [Column("phone")]
        public string Phone { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class Employee
    {
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [Column("position")]
        public string Position { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class Service
    {
        [Column("service_id")]
        public int ServiceId { get; set; }

        [Column("service_name")]
        public string ServiceName { get; set; } = null!;

        [Column("base_price")]
        public decimal BasePrice { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public class Order
    {
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("client_id")]
        public int? ClientId { get; set; }

        [Column("employee_id")]
        public int? EmployeeId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("status")]
        public string Status { get; set; } = null!;

        public Client? Client { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public class OrderDetail
    {
        [Column("detail_id")]
        public int DetailId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("service_id")]
        public int? ServiceId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        public Order Order { get; set; } = null!;
        public Service? Service { get; set; }
    }

    public class AlphabetContext : DbContext
    {
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        public AlphabetContext() { }

        public AlphabetContext(DbContextOptions<AlphabetContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PC;Database=Алфавит;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("Order_Details");

            modelBuilder.Entity<OrderDetail>().HasKey(od => od.DetailId);

            modelBuilder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasColumnType("decimal(10, 2)");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}