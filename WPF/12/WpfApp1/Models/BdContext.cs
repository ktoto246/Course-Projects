using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Models;

public partial class BdContext : DbContext
{
    public BdContext()
    {
    }

    public BdContext(DbContextOptions<BdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Аренда> Арендаs { get; set; }

    public virtual DbSet<Категории> Категорииs { get; set; }

    public virtual DbSet<Клиенты> Клиентыs { get; set; }

    public virtual DbSet<Плавсредства> Плавсредстваs { get; set; }

    public virtual DbSet<Поломки> Поломкиs { get; set; }

    public virtual DbSet<Сотрудники> Сотрудникиs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PC;Database=bd;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Аренда>(entity =>
        {
            entity.HasKey(e => e.IdАренды).HasName("PK__Аренда__1475B6F758E790B9");

            entity.ToTable("Аренда");

            entity.Property(e => e.IdАренды).HasColumnName("ID_Аренды");
            entity.Property(e => e.IdКлиента).HasColumnName("ID_Клиента");
            entity.Property(e => e.IdПлавсредства).HasColumnName("ID_Плавсредства");
            entity.Property(e => e.IdСотрудника).HasColumnName("ID_Сотрудника");
            entity.Property(e => e.ВремяКонца)
                .HasColumnType("datetime")
                .HasColumnName("Время_конца");
            entity.Property(e => e.ВремяНачала)
                .HasColumnType("datetime")
                .HasColumnName("Время_начала");
            entity.Property(e => e.ИтогоКОплате)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Итого_к_оплате");
            entity.Property(e => e.СуммаЗалога)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Сумма_Залога");

            entity.HasOne(d => d.IdКлиентаNavigation).WithMany(p => p.Арендаs)
                .HasForeignKey(d => d.IdКлиента)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Аренда__ID_Клиен__571DF1D5");

            entity.HasOne(d => d.IdПлавсредстваNavigation).WithMany(p => p.Арендаs)
                .HasForeignKey(d => d.IdПлавсредства)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Аренда__ID_Плавс__5812160E");

            entity.HasOne(d => d.IdСотрудникаNavigation).WithMany(p => p.Арендаs)
                .HasForeignKey(d => d.IdСотрудника)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Аренда__ID_Сотру__59063A47");
        });

        modelBuilder.Entity<Категории>(entity =>
        {
            entity.HasKey(e => e.IdКатегории).HasName("PK__Категори__B610B57F623D4901");

            entity.ToTable("Категории");

            entity.Property(e => e.IdКатегории).HasColumnName("ID_Категории");
            entity.Property(e => e.Название).HasMaxLength(50);
            entity.Property(e => e.ЦенаЗаЧас)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Цена_за_час");
        });

        modelBuilder.Entity<Клиенты>(entity =>
        {
            entity.HasKey(e => e.IdКлиента).HasName("PK__Клиенты__F7300111EA639F7D");

            entity.ToTable("Клиенты");

            entity.Property(e => e.IdКлиента).HasColumnName("ID_Клиента");
            entity.Property(e => e.Паспорт).HasMaxLength(100);
            entity.Property(e => e.Телефон).HasMaxLength(20);
            entity.Property(e => e.Фио)
                .HasMaxLength(100)
                .HasColumnName("ФИО");
        });

        modelBuilder.Entity<Плавсредства>(entity =>
        {
            entity.HasKey(e => e.IdПлавсредства).HasName("PK__Плавсред__766F59FB46428C6C");

            entity.ToTable("Плавсредства");

            entity.HasIndex(e => e.СерийныйНомер, "UQ__Плавсред__7BEC59113449C90D").IsUnique();

            entity.Property(e => e.IdПлавсредства).HasColumnName("ID_Плавсредства");
            entity.Property(e => e.IdКатегории).HasColumnName("ID_Категории");
            entity.Property(e => e.Доступно).HasDefaultValue(true);
            entity.Property(e => e.Модель).HasMaxLength(100);
            entity.Property(e => e.СерийныйНомер)
                .HasMaxLength(50)
                .HasColumnName("Серийный_номер");
            entity.Property(e => e.Состояние)
                .HasMaxLength(50)
                .HasDefaultValue("Исправно");

            entity.HasOne(d => d.IdКатегорииNavigation).WithMany(p => p.Плавсредстваs)
                .HasForeignKey(d => d.IdКатегории)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Плавсредс__ID_Ка__534D60F1");
        });

        modelBuilder.Entity<Поломки>(entity =>
        {
            entity.HasKey(e => e.IdПоломки).HasName("PK__Поломки__87E4B86FB2624638");

            entity.ToTable("Поломки");

            entity.Property(e => e.IdПоломки).HasColumnName("ID_Поломки");
            entity.Property(e => e.IdАренды).HasColumnName("ID_Аренды");
            entity.Property(e => e.Описание).HasMaxLength(255);
            entity.Property(e => e.Оплачено).HasDefaultValue(false);
            entity.Property(e => e.СуммаШтрафа)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Сумма_Штрафа");

            entity.HasOne(d => d.IdАрендыNavigation).WithMany(p => p.Поломкиs)
                .HasForeignKey(d => d.IdАренды)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Поломки__ID_Арен__5CD6CB2B");
        });

        modelBuilder.Entity<Сотрудники>(entity =>
        {
            entity.HasKey(e => e.IdСотрудника).HasName("PK__Сотрудни__F4052FE3CB494447");

            entity.ToTable("Сотрудники");

            entity.Property(e => e.IdСотрудника).HasColumnName("ID_Сотрудника");
            entity.Property(e => e.Должность).HasMaxLength(50);
            entity.Property(e => e.Фио)
                .HasMaxLength(100)
                .HasColumnName("ФИО");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
