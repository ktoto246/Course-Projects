using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("УстановкиПО")]
    public class УстановкаПО
    {
        [Key]
        [Column("IDУстановки")]
        public int Id { get; set; }

        [Column("IDПО")]
        public int ПрограммноеОбеспечениеId { get; set; }

        [ForeignKey(nameof(ПрограммноеОбеспечениеId))]
        public ПрограммноеОбеспечение? ПрограммноеОбеспечение { get; set; }

        [Column("IDМеста")]
        public int РабочееМестоId { get; set; }

        [ForeignKey(nameof(РабочееМестоId))]
        public РабочееМесто? РабочееМесто { get; set; }

        [Column("ДатаУстановки")]
        public DateTime? ДатаУстановки { get; set; }
    }
}