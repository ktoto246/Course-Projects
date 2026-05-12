using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class StatusAnalyticsPage : Page
    {
        public ISeries[] StatusSeries { get; set; }

        public StatusAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();

            var stats = db.Equipments
                .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                .GroupBy(e => e.Status.Name)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToList();

            if (stats == null || !stats.Any())
            {
                ChartBorder.Visibility = Visibility.Collapsed;
                txtNoData.Visibility = Visibility.Visible;
                return;
            }

            ChartBorder.Visibility = Visibility.Visible;
            txtNoData.Visibility = Visibility.Collapsed;

            var series = new List<ISeries>();
            foreach (var stat in stats)
            {
                series.Add(new PieSeries<int>
                {
                    Values = new[] { stat.Count },
                    Name = stat.StatusName,
                    InnerRadius = 50
                });
            }

            StatusSeries = series.ToArray();

            DataContext = null;
            DataContext = this;
        }
    }
}