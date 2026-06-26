using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("АппаратноеОбеспечение")]
    public class АппаратноеОбеспечение
    {
        [Key]
        [Column("IDОборудования")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Наименование")]
        public string Название { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("Производитель")]
        public string? Производитель { get; set; }

        [MaxLength(150)]
        [Column("Модель")]
        public string? Модель { get; set; }

        [MaxLength(100)]
        [Column("СерийныйНомер")]
        public string? СерийныйНомер { get; set; }

        [Column("ДатаПриобретения")]
        public DateTime? ДатаПриобретения { get; set; }

        [Column("Стоимость")]
        public decimal? Стоимость { get; set; }

        [Column("IDМеста")]
        public int? РабочееМестоId { get; set; }

        [ForeignKey(nameof(РабочееМестоId))]
        public РабочееМесто? РабочееМесто { get; set; }
    }
}