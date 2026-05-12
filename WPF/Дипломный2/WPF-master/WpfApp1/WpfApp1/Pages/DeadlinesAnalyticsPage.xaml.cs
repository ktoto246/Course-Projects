using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DeadlinesAnalyticsPage : Page
    {
        public DeadlinesAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            var horizon = DateTime.Today.AddMonths(6);
            var list = db.Equipments.Include(e => e.Category)
                .Where(e => e.Status.Name != SystemStatuses.Scrapped &&
                            (e.WarrantyExpireDate <= horizon || (e.PurchaseDate.HasValue && e.PurchaseDate.Value.AddYears(e.ServiceLifeYears ?? 5) <= horizon)))
                .ToList();
            DeadlinesGrid.ItemsSource = list;
        }
    }
}