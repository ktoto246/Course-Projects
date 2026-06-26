using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ModelName { get; set; } = null!;
        public decimal Height { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
