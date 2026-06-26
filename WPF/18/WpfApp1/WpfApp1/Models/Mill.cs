using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Mill
{
    public int MillId { get; set; }
    public string MillName { get; set; } = null!;
    public int YearBuilt { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}