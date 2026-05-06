using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class FinancialAnalyticsPage : Page
    {
        public ISeries[] FinancialSeries { get; set; }
        public Axis[] XAxes { get; set; }

        public FinancialAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            var stats = db.Equipments
                .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                .GroupBy(e => e.Category.Name)
                .Select(g => new { Name = g.Key, Total = g.Sum(e => e.Price ?? 0) })
                .OrderByDescending(x => x.Total).ToList();

            if (!stats.Any()) { ChartBorder.Visibility = Visibility.Collapsed; txtNoData.Visibility = Visibility.Visible; return; }

            ChartBorder.Visibility = Visibility.Visible; txtNoData.Visibility = Visibility.Collapsed;
            FinancialSeries = new ISeries[] { new ColumnSeries<decimal> { Values = stats.Select(s => s.Total).ToArray(), Name = "Сумма (руб.)" } };
            XAxes = new Axis[] { new Axis { Labels = stats.Select(s => s.Name).ToArray() } };
            DataContext = null; DataContext = this;
        }
    }
}