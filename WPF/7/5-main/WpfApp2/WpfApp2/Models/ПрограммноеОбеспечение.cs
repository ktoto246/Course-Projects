using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("ПрограммноеОбеспечение")]
    public class ПрограммноеОбеспечение
    {
        [Key]
        [Column("IDПО")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("Наименование")]
        public string Название { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Версия { get; set; }

        [MaxLength(150)]
        public string? Разработчик { get; set; }

        [MaxLength(100)]
        public string? ТипЛицензии { get; set; }

        [MaxLength(200)]
        [Column("ЛицензионныйКлюч")]
        public string? ЛицензионныйКлюч { get; set; }

        public ICollection<УстановкаПО> Установки { get; set; } = new List<УстановкаПО>();
    }
}