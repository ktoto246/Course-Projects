using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Аренда
{
    public int IdАренды { get; set; }

    public int IdКлиента { get; set; }

    public int IdПлавсредства { get; set; }

    public int IdСотрудника { get; set; }

    public DateTime ВремяНачала { get; set; }

    public DateTime? ВремяКонца { get; set; }

    public decimal? СуммаЗалога { get; set; }

    public decimal? ИтогоКОплате { get; set; }

    public virtual Клиенты IdКлиентаNavigation { get; set; } = null!;

    public virtual Плавсредства IdПлавсредстваNavigation { get; set; } = null!;

    public virtual Сотрудники IdСотрудникаNavigation { get; set; } = null!;

    public virtual ICollection<Поломки> Поломкиs { get; set; } = new List<Поломки>();
}
