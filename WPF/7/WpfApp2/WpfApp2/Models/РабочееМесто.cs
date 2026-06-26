using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("РабочиеМеста")]
    public class РабочееМесто
    {
        [Key]
        [Column("IDМеста")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ИнвентарныйНомер")]
        public string ИнвентарныйНомер { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("Расположение")]
        public string? Расположение { get; set; }

        [Column("IDОтдела")] 
        public int ОтделId { get; set; }

        [ForeignKey(nameof(ОтделId))]
        public Отдел? Отдел { get; set; }

        public ICollection<АппаратноеОбеспечение> Оборудование { get; set; } = new List<АппаратноеОбеспечение>();
        public ICollection<УстановкаПО> УстановкиПО { get; set; } = new List<УстановкаПО>();
    }
}