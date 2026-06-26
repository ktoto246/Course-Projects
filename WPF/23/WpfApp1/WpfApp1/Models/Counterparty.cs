using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Counterparties")]
    public class Counterparty
    {
        [Key]
        public int CounterpartyID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = null!;

        [StringLength(12)]
        public string? INN { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public ICollection<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();
    }
}