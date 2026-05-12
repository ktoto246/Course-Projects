using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("Сотрудники")]
    public class Employee
    {
        [Key]
        [Column("ID_Сотрудника")]
        public int Id { get; set; }

        [Required]
        [Column("ФИО")]
        public string FullName { get; set; } = null!;

        [Required]
        [Column("Логин")]
        public string Login { get; set; } = null!;

        [Required]
        [Column("Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        [Column("Роль")]
        public string Role { get; set; } = null!;

        [Column("Статус_Активности")]
        public bool IsActive { get; set; } = true;

        public ICollection<GrainBatch> WeighedBatches { get; set; } = new List<GrainBatch>();
        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
        public ICollection<RenderedService> ProcessedServices { get; set; } = new List<RenderedService>();
    }

    [Table("Клиенты")]
    public class Client
    {
        [Key]
        [Column("ID_Клиента")]
        public int Id { get; set; }

        [Required]
        [Column("Название_Организации")]
        public string CompanyName { get; set; } = null!;

        [Required]
        [Column("ИНН")]
        public string INN { get; set; } = null!;

        [Column("Адрес")]
        public string? Address { get; set; }

        [Column("Контактный_Телефон")]
        public string? Phone { get; set; }

        public ICollection<GrainBatch> Batches { get; set; } = new List<GrainBatch>();
    }

    [Table("Справочник_Культур")]
    public class Crop
    {
        [Key]
        [Column("ID_Культуры")]
        public int Id { get; set; }

        [Required]
        [Column("Название_Культуры")]
        public string Name { get; set; } = null!;

        [Column("Базовая_Влажность_Процент")]
        public decimal BaseMoisture { get; set; }

        [Column("Базовая_Сорность_Процент")]
        public decimal BaseWeediness { get; set; }

        public ICollection<GrainBatch> Batches { get; set; } = new List<GrainBatch>();
    }

    [Table("Склады")]
    public class Storage
    {
        [Key]
        [Column("ID_Склада")]
        public int Id { get; set; }

        [Required]
        [Column("Наименование_Склада")]
        public string Name { get; set; } = null!;

        [Column("Вместимость_Тонн")]
        public decimal Capacity { get; set; }

        [Column("Текущая_Загрузка_Тонн")]
        public decimal CurrentLoad { get; set; } = 0;

        public ICollection<GrainBatch> Batches { get; set; } = new List<GrainBatch>();
    }

    [Table("Услуги")]
    public class Service
    {
        [Key]
        [Column("ID_Услуги")]
        public int Id { get; set; }

        [Required]
        [Column("Название_Услуги")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("Единица_Измерения")]
        public string Unit { get; set; } = null!;

        [Column("Цена_За_Единицу")]
        public decimal UnitPrice { get; set; }

        public ICollection<RenderedService> RenderedServices { get; set; } = new List<RenderedService>();
    }

    [Table("Партии_Зерна")]
    public class GrainBatch
    {
        [Key]
        [Column("ID_Партии")]
        public int Id { get; set; }

        [Column("ID_Клиента")]
        public int ClientId { get; set; }

        [Column("ID_Весовщика")]
        public int WeigherId { get; set; }

        [Column("ID_Культуры")]
        public int CropId { get; set; }

        [Column("ID_Склада")]
        public int? StorageId { get; set; }

        [Required]
        [Column("Номер_Авто")]
        public string CarNumber { get; set; } = null!;

        [Column("Вес_Брутто")]
        public decimal GrossWeight { get; set; }

        [Column("Вес_Тара")]
        public decimal TareWeight { get; set; }

        // Это поле вычисляемое в БД, в C# мы его только читаем
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("Вес_Нетто")]
        public decimal NetWeight { get; private set; }

        [Column("Дата_Прибытия")]
        public DateTime ArrivalDate { get; set; } = DateTime.Now;

        [Column("Статус")]
        public string Status { get; set; } = "Ожидает анализа";

        // Навигация
        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

        [ForeignKey("WeigherId")]
        public Employee Weigher { get; set; } = null!;

        [ForeignKey("CropId")]
        public Crop Crop { get; set; } = null!;

        [ForeignKey("StorageId")]
        public Storage? Storage { get; set; }

        public LabTest? LabTest { get; set; }
        public ICollection<RenderedService> RenderedServices { get; set; } = new List<RenderedService>();
    }

    [Table("Лабораторные_Анализы")]
    public class LabTest
    {
        [Key]
        [Column("ID_Анализа")]
        public int Id { get; set; }

        [Column("ID_Партии")]
        public int BatchId { get; set; }

        [Column("ID_Лаборанта")]
        public int LabTechId { get; set; }

        [Column("Влажность_Факт")]
        public decimal Moisture { get; set; }

        [Column("Сорность_Факт")]
        public decimal Weediness { get; set; }

        [Column("Результат_Органолептики")]
        public string? Organoleptics { get; set; }

        [Column("Дата_Анализа")]
        public DateTime TestDate { get; set; } = DateTime.Now;

        [ForeignKey("BatchId")]
        public GrainBatch Batch { get; set; } = null!;

        [ForeignKey("LabTechId")]
        public Employee LabTech { get; set; } = null!;
    }

    [Table("Оказанные_Услуги")]
    public class RenderedService
    {
        [Key]
        [Column("ID_Записи")]
        public int Id { get; set; }

        [Column("ID_Партии")]
        public int BatchId { get; set; }

        [Column("ID_Услуги")]
        public int ServiceId { get; set; }

        [Column("ID_Менеджера")]
        public int ManagerId { get; set; }

        [Column("Количество")]
        public decimal Quantity { get; set; }

        [Column("Итоговая_Стоимость")]
        public decimal TotalPrice { get; set; }

        [Column("Дата_Оформления")]
        public DateTime RecordDate { get; set; } = DateTime.Now;

        [ForeignKey("BatchId")]
        public GrainBatch Batch { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public Service Service { get; set; } = null!;

        [ForeignKey("ManagerId")]
        public Employee Manager { get; set; } = null!;
    }
}