using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public class BalashovDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<IssuanceLog> IssuanceLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=PC;Database=Balashov_BoardingHouse;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IssuanceLog>().ToTable("IssuanceLog");

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IssuanceLog>()
                .HasOne(l => l.Resident)
                .WithMany(r => r.IssuanceLogs)
                .HasForeignKey(l => l.ResidentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssuanceLog>()
                .HasOne(l => l.Item)
                .WithMany(i => i.IssuanceLogs)
                .HasForeignKey(l => l.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssuanceLog>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.IssuanceLogs)
                .HasForeignKey(l => l.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class Employee
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }

        public ICollection<IssuanceLog> IssuanceLogs { get; set; } = new List<IssuanceLog>();
    }

    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }

    public class Resident
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string RoomNumber { get; set; }
        public int? DisabilityGroup { get; set; }

        public ICollection<IssuanceLog> IssuanceLogs { get; set; } = new List<IssuanceLog>();
    }

    public class Item
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int CategoryID { get; set; }

        public Category Category { get; set; }

        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public ICollection<IssuanceLog> IssuanceLogs { get; set; } = new List<IssuanceLog>();
    }

    public class IssuanceLog
    {
        public int ID { get; set; }
        public int ResidentID { get; set; }
        public Resident Resident { get; set; }

        public int ItemID { get; set; }
        public Item Item { get; set; }

        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Quantity { get; set; }
    }
}