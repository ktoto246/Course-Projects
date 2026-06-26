using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Crops")]
    public class Crop
    {
        [Key]
        public int CropID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!;

        [Required]
        public int MaturationDays { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SeedCostPerHa { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal? ExpectedYieldTHa { get; set; }

        public ICollection<Plot> Plots { get; set; } = new List<Plot>();
    }
}