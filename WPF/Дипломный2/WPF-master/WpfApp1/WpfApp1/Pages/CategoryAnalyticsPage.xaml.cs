using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class CategoryAnalyticsPage : Page
    {
        public ISeries[] CategorySeries { get; set; }
        public Axis[] XAxes { get; set; }

        public CategoryAnalyticsPage()
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
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToList();

            if (stats == null || !stats.Any())
            {
                ChartBorder.Visibility = Visibility.Collapsed;
                txtNoData.Visibility = Visibility.Visible;
                return;
            }

            ChartBorder.Visibility = Visibility.Visible;
            txtNoData.Visibility = Visibility.Collapsed;

            CategorySeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = stats.Select(s => s.Count).ToArray(),
                    Name = "Количество (шт.)"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = stats.Select(s => s.CategoryName).ToArray(),
                    LabelsRotation = 15
                }
            };

            DataContext = null;
            DataContext = this;
        }
    }
}