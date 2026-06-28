using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;

        public List<Inspection> Inspections { get; set; } = new();
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string DriverLicense { get; set; } = null!;

        public List<Vehicle> Vehicles { get; set; } = new();
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string VIN { get; set; } = null!;
        public string RegNumber { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Category { get; set; } = null!;

        public Client Client { get; set; } = null!;
        public List<Inspection> Inspections { get; set; } = new();
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public List<Inspection> Inspections { get; set; } = new();
    }

    public class Inspection
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int EmployeeId { get; set; }
        public int ServiceId { get; set; }
        public DateTime InspectionDate { get; set; }
        public bool IsPassed { get; set; }
        public string? Comments { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }

    public class BmtcContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Inspection> Inspections { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=BMTC_DB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicles");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Inspection>().ToTable("Inspections");

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Client)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.ClientId);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Vehicle)
                .WithMany(v => v.Inspections)
                .HasForeignKey(i => i.VehicleId);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Employee)
                .WithMany(e => e.Inspections)
                .HasForeignKey(i => i.EmployeeId);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Service)
                .WithMany(s => s.Inspections)
                .HasForeignKey(i => i.ServiceId);
        }
    }
}