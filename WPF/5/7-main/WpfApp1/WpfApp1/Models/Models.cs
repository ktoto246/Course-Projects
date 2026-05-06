using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Сотрудники")]
    public class Employee
    {
        [Key]
        [Column("ID_Сотрудника")]
        public int Id { get; set; }

        [Column("ФИО")]
        public string FullName { get; set; } = null!;

        [Column("Должность")]
        public string Position { get; set; } = null!;

        [Column("Отдел")]
        public string Department { get; set; } = null!;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }

    [Table("Клиенты")]
    public class Client
    {
        [Key]
        [Column("ID_Клиента")]
        public int Id { get; set; }

        [Column("Название_Компании")]
        public string CompanyName { get; set; } = null!;

        [Column("Контактное_Лицо")]
        public string ContactPerson { get; set; } = null!;

        [Column("Телефон")]
        public string Phone { get; set; } = null!;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }

    [Table("Продукты")]
    public class Product
    {
        [Key]
        [Column("ID_Продукта")]
        public int Id { get; set; }

        [Column("Название_Продукта")]
        public string ProductName { get; set; } = null!;

        [Column("Категория")]
        public string Category { get; set; } = null!;

        [Column("Базовая_Цена")]
        public decimal BasePrice { get; set; }

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }

    [Table("Сделки")]
    public class Deal
    {
        [Key]
        [Column("ID_Сделки")]
        public int Id { get; set; }

        [Column("ID_Клиента")]
        public int ClientId { get; set; }

        [Column("ID_Сотрудника")]
        public int EmployeeId { get; set; }

        [Column("ID_Продукта")]
        public int ProductId { get; set; }

        [Column("Количество")]
        public int Quantity { get; set; }

        [Column("Итоговая_Сумма")]
        public decimal TotalAmount { get; set; }

        [Column("Дата_Сделки")]
        public DateTime DealDate { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}