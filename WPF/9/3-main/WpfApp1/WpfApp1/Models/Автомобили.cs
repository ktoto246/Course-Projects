using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models;

[Table("Автомобили")]
public partial class Автомобили
{
    [Key]
    [Column("ID_Авто")]
    public int IdАвто { get; set; }

    [Column("Гос_Номер")]
    public string ГосНомер { get; set; } = null!;

    [Column("Марка_Модель")]
    public string МаркаМодель { get; set; } = null!;

    [Column("Год_Выпуска")]
    public int ГодВыпуска { get; set; }

    [Column("Текущий_Пробег")]
    public int? ТекущийПробег { get; set; }

    [Column("Статус")]
    public string? Статус { get; set; }

    // Навигационные свойства для связанных таблиц
    public virtual ICollection<Автопробег> Автопробеги { get; set; } = new List<Автопробег>();
    public virtual ICollection<ПлановыеРемонты> ПлановыеРемонтыs { get; set; } = new List<ПлановыеРемонты>();
}