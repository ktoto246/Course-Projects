using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Client
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string? Phone { get; set; }
    public string? LegalAddress { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}