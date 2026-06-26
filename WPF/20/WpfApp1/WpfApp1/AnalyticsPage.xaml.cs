using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class AnalyticsPage : Page
    {
        private BalashovDbContext _context;

        public AnalyticsPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadCharts();
        }

        private void LoadCharts()
        {
            if (_context == null) return;

            var categoryStats = _context.Items
                .Include(i => i.Category)
                .GroupBy(i => i.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Sum(i => i.TotalQuantity) })
                .ToList();

            SeriesCollection pieSeries = new SeriesCollection();
            foreach (var stat in categoryStats)
            {
                pieSeries.Add(new PieSeries
                {
                    Title = stat.Category,
                    Values = new ChartValues<int> { stat.Count },
                    DataLabels = true
                });
            }
            ChartCategories.Series = pieSeries;

            var topItems = _context.IssuanceLogs
                .Include(l => l.Item)
                .GroupBy(l => l.Item.ItemName)
                .Select(g => new { ItemName = g.Key, IssuedCount = g.Sum(l => l.Quantity) })
                .OrderByDescending(x => x.IssuedCount)
                .Take(5)
                .ToList();

            ChartTopItems.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Выдано (шт.)",
                    Values = new ChartValues<int>(topItems.Select(x => x.IssuedCount))
                }
            };

            AxisX.Labels = topItems.Select(x => x.ItemName).ToList();
        }
    }
}