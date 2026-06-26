using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
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
            try
            {
                using var db = new AppDbContext();
                var horizon = DateTime.Today.AddMonths(6);
        
                var candidates = db.Equipments
                    .Include(e => e.Category)
                    .Include(e => e.Status)
                    .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                    .ToList(); 

                var list = candidates.Where(e =>
                    (e.WarrantyExpireDate.HasValue && e.WarrantyExpireDate <= horizon) ||
                    (e.PurchaseDate.HasValue && e.ServiceLifeYears.GetValueOrDefault(5) > 0 && e.PurchaseDate.Value.AddYears(e.ServiceLifeYears.GetValueOrDefault(5)) <= horizon)
                ).ToList();
        
                DeadlinesGrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки дедлайнов:\n{ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}