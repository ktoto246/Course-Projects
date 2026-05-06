using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ТипСвт> ТипыСвт { get; set; }
        public DbSet<Сотрудник> Сотрудники { get; set; }
        public DbSet<Техника> Техника { get; set; }
        public DbSet<Комплектующее> Комплектующие { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=PC;Database=Учет_СВТ_Оптима;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}