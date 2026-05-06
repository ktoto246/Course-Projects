using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("Нефтепродукты")]
    public class Нефтепродукт
    {
        [Key]
        public int ID_Продукта { get; set; }

        [Required]
        [MaxLength(255)]
        public string Название { get; set; } = null!;

        public int Класс_Опасности { get; set; }

        public decimal Цена_За_Тонну { get; set; }

        public ICollection<Поступление> Поступления { get; set; } = new List<Поступление>();
        public ICollection<Реализация> Реализации { get; set; } = new List<Реализация>();
    }

    [Table("Клиенты")]
    public class Клиент
    {
        [Key]
        public int ID_Клиента { get; set; }

        [Required]
        [MaxLength(255)]
        public string Название_Компании { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ИНН { get; set; } = null!;

        [MaxLength(50)]
        public string? Контактный_Телефон { get; set; }

        public ICollection<Реализация> Реализации { get; set; } = new List<Реализация>();
    }

    [Table("Поступления")]
    public class Поступление
    {
        [Key]
        public int ID_Поступления { get; set; }

        public int ID_Продукта { get; set; }

        public decimal Количество_Тонн { get; set; }

        public DateTime Дата_Поступления { get; set; }

        [ForeignKey("ID_Продукта")]
        public Нефтепродукт Нефтепродукт { get; set; } = null!;
    }

    [Table("Реализация")]
    public class Реализация
    {
        [Key]
        public int ID_Реализации { get; set; }

        public int ID_Продукта { get; set; }

        public int ID_Клиента { get; set; }

        public decimal Количество_Тонн { get; set; }

        public decimal Общая_Стоимость { get; set; }

        public DateTime Дата_Отгрузки { get; set; }

        [ForeignKey("ID_Продукта")]
        public Нефтепродукт Нефтепродукт { get; set; } = null!;

        [ForeignKey("ID_Клиента")]
        public Клиент Клиент { get; set; } = null!;
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Нефтепродукт> Нефтепродукты { get; set; }
        public DbSet<Клиент> Клиенты { get; set; }
        public DbSet<Поступление> Поступления { get; set; }
        public DbSet<Реализация> Реализации { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PC;Database=УчетНефтебазы;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Нефтепродукт>()
                .Property(n => n.Цена_За_Тонну)
                .HasColumnType("decimal(12, 2)");

            modelBuilder.Entity<Поступление>()
                .Property(p => p.Количество_Тонн)
                .HasColumnType("decimal(12, 2)");

            modelBuilder.Entity<Реализация>()
                .Property(r => r.Количество_Тонн)
                .HasColumnType("decimal(12, 2)");

            modelBuilder.Entity<Реализация>()
                .Property(r => r.Общая_Стоимость)
                .HasColumnType("decimal(12, 2)");
        }
    }
}