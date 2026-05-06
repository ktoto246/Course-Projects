using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Плавсредства
{
    public int IdПлавсредства { get; set; }

    public int IdКатегории { get; set; }

    public string Модель { get; set; } = null!;

    public string СерийныйНомер { get; set; } = null!;

    public string? Состояние { get; set; }

    public bool? Доступно { get; set; }

    public virtual Категории IdКатегорииNavigation { get; set; } = null!;

    public virtual ICollection<Аренда> Арендаs { get; set; } = new List<Аренда>();
}
