using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1.Models
{
    public class Client
    {
        public int ClientID { get; set; }
        public string CompanyName { get; set; } = null!;
        public string INN { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string City { get; set; } = "Балашов";
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
