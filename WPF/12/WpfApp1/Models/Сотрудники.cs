using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Сотрудники
{
    public int IdСотрудника { get; set; }

    public string Фио { get; set; } = null!;

    public string Должность { get; set; } = null!;

    public virtual ICollection<Аренда> Арендаs { get; set; } = new List<Аренда>();
}
