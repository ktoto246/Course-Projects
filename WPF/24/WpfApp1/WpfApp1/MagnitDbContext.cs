using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public class MagnitDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=MagnitDeliveryDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public List<Product> Products { get; set; } = new();
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;

        public List<Delivery> Deliveries { get; set; } = new();
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Inn { get; set; } = null!;

        public List<Delivery> Deliveries { get; set; } = new();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
        public List<Delivery> Deliveries { get; set; } = new();
    }

    public class Delivery
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int EmployeeId { get; set; }

        public Product Product { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}