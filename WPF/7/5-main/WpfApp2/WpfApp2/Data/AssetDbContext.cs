using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WpfApp1.Models;

namespace WpfApp1.Data
{
    public class AssetDbContext : DbContext
    {
        public DbSet<Отдел> Отделы { get; set; }
        public DbSet<РабочееМесто> РабочиеМеста { get; set; }
        public DbSet<АппаратноеОбеспечение> АппаратноеОбеспечение { get; set; }
        public DbSet<ПрограммноеОбеспечение> ПрограммноеОбеспечение { get; set; }
        public DbSet<УстановкаПО> УстановкиПО { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PC;Database=AssetDB;Trusted_Connection=True;Encrypt=False;");
            }
        }
    }
}