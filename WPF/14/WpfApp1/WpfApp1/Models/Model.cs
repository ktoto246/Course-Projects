using System;
using System.Collections.Generic;

namespace WpfApp1.Models
{
    public class ProductionUnit
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }

    public class Manufacturer
    {
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; } = string.Empty;
        public string? Country { get; set; }
        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }

    public class EquipmentType
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }

    public class EquipmentStatus
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    }

    public class Equipment
    {
        public int EquipmentID { get; set; }
        public string InventoryNumber { get; set; } = string.Empty;
        public string EquipmentName { get; set; } = string.Empty;
        public int TypeID { get; set; }
        public int ManufacturerID { get; set; }
        public string? Model { get; set; }
        public int UnitID { get; set; }
        public int StatusID { get; set; }
        public DateOnly? InstallDate { get; set; }
        public DateOnly? LastMaintenanceDate { get; set; }
        public DateOnly? NextMaintenanceDate { get; set; }
        public string? Notes { get; set; }
        public EquipmentType Type { get; set; } = null!;
        public Manufacturer Manufacturer { get; set; } = null!;
        public ProductionUnit Unit { get; set; } = null!;
        public EquipmentStatus Status { get; set; } = null!;
        public DateOnly? WriteOffDate { get; set; }
        public decimal? WriteOffRepairCost { get; set; }
        public decimal? WriteOffPartsCost { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }

    public class MaintenanceRecord
    {
        public int RecordID { get; set; }
        public int EquipmentID { get; set; }
        public DateOnly MaintenanceDate { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal Cost { get; set; }

        public int EmployeeID { get; set; }
        public Employee Employee { get; set; } = null!;

        public Equipment Equipment { get; set; } = null!;
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }
}