using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Категории
{
    public int IdКатегории { get; set; }

    public string Название { get; set; } = null!;

    public decimal ЦенаЗаЧас { get; set; }

    public virtual ICollection<Плавсредства> Плавсредстваs { get; set; } = new List<Плавсредства>();
}
