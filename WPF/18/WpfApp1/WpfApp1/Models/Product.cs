using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal PackagingWeightKg { get; set; }
    public decimal PricePerTon { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}