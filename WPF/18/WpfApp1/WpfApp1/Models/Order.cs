using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models;

public partial class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public int ClientId { get; set; }
    public int MillId { get; set; }
    public virtual Client Client { get; set; } = null!;
    public virtual Mill Mill { get; set; } = null!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [NotMapped]
    public decimal TotalSum => OrderDetails.Sum(od => od.QuantityTons * od.Product.PricePerTon);
}