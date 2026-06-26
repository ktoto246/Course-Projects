using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("FinancialTransactions")]
    public class FinancialTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Type { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public Employee Employee { get; set; } = null!;

        public int? PlotID { get; set; }

        [ForeignKey(nameof(PlotID))]
        public Plot? Plot { get; set; }

        public int? CounterpartyID { get; set; }

        [ForeignKey(nameof(CounterpartyID))]
        public Counterparty? Counterparty { get; set; }
    }
}