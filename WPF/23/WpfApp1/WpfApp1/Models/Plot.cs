using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Plots")]
    public class Plot
    {
        [Key]
        public int PlotID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public int ResponsibleEmployeeID { get; set; }

        [ForeignKey(nameof(ResponsibleEmployeeID))]
        public Employee ResponsibleEmployee { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AreaHa { get; set; }

        public int? CropID { get; set; }

        [ForeignKey(nameof(CropID))]
        public Crop? Crop { get; set; }

        public DateTime? PlantingDate { get; set; }
        public DateTime? PlannedHarvestDate { get; set; }

        public ICollection<FinancialTransaction> FinancialTransactions { get; set; } = new List<FinancialTransaction>();
    }
}