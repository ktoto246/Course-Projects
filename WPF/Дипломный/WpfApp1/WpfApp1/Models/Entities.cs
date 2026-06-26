using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }

    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }

    public class Position
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Cabinet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Number { get; set; } = null!;

        public int? ResponsibleEmployeeId { get; set; }
        [ForeignKey("ResponsibleEmployeeId")]
        public Employee? ResponsibleEmployee { get; set; }

        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }

    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        public int? PositionId { get; set; }
        [ForeignKey("PositionId")]
        public Position? Position { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }

    [Index(nameof(InventoryNumber), IsUnique = true)]
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string InventoryNumber { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? Price { get; set; }

        public int? CabinetId { get; set; }
        [ForeignKey("CabinetId")]
        public Cabinet? Cabinet { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; } = null!;

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public ICollection<RepairHistory> RepairHistories { get; set; } = new List<RepairHistory>();

        [NotMapped]
        public decimal CurrentAmortizedValue
        {
            get
            {
                if (!Price.HasValue || !PurchaseDate.HasValue) return Price ?? 0m;
                if (PurchaseDate.Value > DateTime.Today) return Price.Value;

                int usefulLife = ServiceLifeYears ?? 5;
                if (usefulLife <= 0) return 0m;

                decimal yearsElapsed = (decimal)(DateTime.Today - PurchaseDate.Value).TotalDays / 365.25m;
                decimal ratio = Math.Min(yearsElapsed / usefulLife, 1.0m);
                decimal depreciated = Price.Value * ratio;

                return Math.Max(Price.Value - depreciated, 0m);
            }
        }

        [NotMapped]
        public decimal TotalRepairCost => RepairHistories != null ? RepairHistories.Sum(r => r.Cost ?? 0) : 0;

        public DateTime? WarrantyExpireDate { get; set; }
        public int? ServiceLifeYears { get; set; }
        [MaxLength(200)]
        public string? Supplier { get; set; }
    }

    public class MovementHistory
    {
        [Key]
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        [ForeignKey("EquipmentId")]
        public Equipment Equipment { get; set; } = null!;

        public int? FromEmployeeId { get; set; }
        [ForeignKey("FromEmployeeId")]
        public Employee? FromEmployee { get; set; }

        public int? ToEmployeeId { get; set; }
        [ForeignKey("ToEmployeeId")]
        public Employee? ToEmployee { get; set; }

        public DateTime TransferDate { get; set; }

        [MaxLength(250)]
        public string? Reason { get; set; }
    }

    public class RepairHistory
    {
        [Key]
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        [ForeignKey("EquipmentId")]
        public Equipment Equipment { get; set; } = null!;

        public DateTime DateIn { get; set; }

        public DateTime? DateOut { get; set; }

        [Required]
        [MaxLength(500)]
        public string IssueDescription { get; set; } = null!;

        public decimal? Cost { get; set; }

        [MaxLength(200)]
        public string? Contractor { get; set; }

        public bool IsWarrantyRepair { get; set; }
    }

    [Index(nameof(Login), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }
    }

    public static class AppSession
    {
        private static string _currentRole;
        private static bool _isLocked = false;

        private static readonly HashSet<string> ValidRoles = new() { "Admin", "Operator", "Viewer" };

        public static string CurrentRole => _currentRole;

        public static void SetRole(string role)
        {
            if (_isLocked) throw new InvalidOperationException("������ ��� ����������������.");
            if (!ValidRoles.Contains(role)) throw new ArgumentException($"������ ��������. ����������� ����: {role}");

            _currentRole = role;
            _isLocked = true;
        }

        public static void Clear()
        {
            _currentRole = null;
            _isLocked = false;
        }
    }

    public static class SystemStatuses
    {
        public const string InUse = "� ������������";
        public const string OnStock = "�� ������";
        public const string InRepair = "� �������";
        public const string Scrapped = "�������";
    }
}