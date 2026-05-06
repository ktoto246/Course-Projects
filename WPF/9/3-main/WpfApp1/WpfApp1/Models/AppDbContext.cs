using Microsoft.EntityFrameworkCore;
using System;

namespace WpfApp1.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Автомобили> Автомобилис { get; set; }
    public virtual DbSet<Автопробег> Автопробеги { get; set; }
    public virtual DbSet<ПлановыеРемонты> ПлановыеРемонтыs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=УчетАвтотранспорта;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Автомобили>(entity =>
        {
            entity.HasIndex(e => e.ГосНомер).IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}