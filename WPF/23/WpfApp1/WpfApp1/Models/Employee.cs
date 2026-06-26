using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; } = null!;

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        // Навигационные свойства
        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
        public ICollection<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();
    }
}