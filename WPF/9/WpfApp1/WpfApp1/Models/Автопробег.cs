using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models;

[Table("Автопробег")]
public partial class Автопробег
{
    [Key]
    [Column("ID_Записи")]
    public int IdЗаписи { get; set; }

    [Column("ID_Авто")]
    public int? IdАвто { get; set; }

    [Column("Дата_Поездки")]
    public DateTime ДатаПоездки { get; set; }

    [Column("Пройдено_Км")]
    public int ПройденоКм { get; set; }

    [Column("Водитель")]
    public string? Водитель { get; set; }

    [Column("Путевой_Лист")]
    public string? ПутевойЛист { get; set; }

    [ForeignKey("IdАвто")]
    public virtual Автомобили? IdАвтоNavigation { get; set; }
}