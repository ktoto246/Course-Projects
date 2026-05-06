using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models;

[Table("Плановые_Ремонты")]
public partial class ПлановыеРемонты
{
    [Key]
    [Column("ID_Ремонта")]
    public int IdРемонта { get; set; }

    [Column("ID_Авто")]
    public int? IdАвто { get; set; }

    [Column("Тип_Ремонта")]
    public string ТипРемонта { get; set; } = null!;

    [Column("Плановая_Дата")]
    public DateTime? ПлановаяДата { get; set; }

    [Column("Плановый_Пробег")]
    public int? ПлановыйПробег { get; set; }

    [Column("Фактическая_Дата")]
    public DateTime? ФактическаяДата { get; set; }

    [Column("Статус")]
    public string? Статус { get; set; }

    [Column("Описание")]
    public string? Описание { get; set; }

    [ForeignKey("IdАвто")]
    public virtual Автомобили? IdАвтоNavigation { get; set; }
}