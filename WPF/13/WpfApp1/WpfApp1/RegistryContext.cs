using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class RegistryContext : DbContext
    {
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=PC;Database=Регистратура_БД;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Specialization)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecializationId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId);
        }
    }

    [Table("Специальности")]
    public class Specialization
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("Название")]
        public string Name { get; set; }

        public List<Doctor> Doctors { get; set; } = new();
    }

    [Table("Врачи")]
    public class Doctor
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("ФИО")]
        public string FullName { get; set; }

        [Column("ID_Специальности")]
        public int SpecializationId { get; set; }

        [Column("Кабинет")]
        public int CabinetNumber { get; set; }

        public Specialization Specialization { get; set; }
    }

    [Table("Пациенты")]
    public class Patient
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("ФИО")]
        public string FullName { get; set; }

        [Column("Дата_Рождения")]
        public DateTime BirthDate { get; set; }

        [Column("Телефон")]
        public string Phone { get; set; }

        [Column("Адрес")]
        public string Address { get; set; }
    }

    [Table("Приемы")]
    public class Appointment
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ID_Пациента")]
        public int PatientId { get; set; }

        [Column("ID_Врача")]
        public int DoctorId { get; set; }

        [Column("Дата_Приема")]
        public DateTime AppointmentDate { get; set; }

        [Column("Статус")]
        public string Status { get; set; } = "Запланировано";

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}