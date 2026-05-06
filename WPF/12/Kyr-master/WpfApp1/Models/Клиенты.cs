using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Клиенты
{
    public int IdКлиента { get; set; }

    public string Фио { get; set; } = null!;

    public string Телефон { get; set; } = null!;

    public string? Паспорт { get; set; }

    public virtual ICollection<Аренда> Арендаs { get; set; } = new List<Аренда>();
}
