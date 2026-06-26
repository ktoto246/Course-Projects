using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ClientID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; } = "Новый";

        public virtual Client Client { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
