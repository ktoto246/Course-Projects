using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Поломки
{
    public int IdПоломки { get; set; }

    public int IdАренды { get; set; }

    public string Описание { get; set; } = null!;

    public decimal СуммаШтрафа { get; set; }

    public bool? Оплачено { get; set; }

    public virtual Аренда IdАрендыNavigation { get; set; } = null!;
}
