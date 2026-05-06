using System;
using System.Linq;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RepairCostsAnalyticsPage : Page
    {
        public ISeries[] RepairSeries { get; set; }
        public Axis[] XAxes { get; set; }

        public RepairCostsAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();

            var startDate = DateTime.Today.AddYears(-3);
            var raw = db.RepairHistories
                        .Where(r => r.DateIn >= startDate)
                        .OrderBy(r => r.DateIn)
                        .ToList();

            if (raw == null || !raw.Any())
            {
                ChartBorder.Visibility = System.Windows.Visibility.Collapsed;
                txtNoData.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            ChartBorder.Visibility = System.Windows.Visibility.Visible;
            txtNoData.Visibility = System.Windows.Visibility.Collapsed;

            var stats = raw.GroupBy(r => new { r.DateIn.Year, r.DateIn.Month })
                           .Select(g => new {
                               DateLabel = $"{g.Key.Month:D2}.{g.Key.Year}",
                               Total = g.Sum(r => r.Cost ?? 0)
                           })
                           .ToList();

            RepairSeries = new ISeries[] {
        new LineSeries<decimal> {
            Values = stats.Select(s => s.Total).ToArray(),
            Name = "Затраты (руб.)"
        }
    };

            XAxes = new Axis[] {
        new Axis { Labels = stats.Select(s => s.DateLabel).ToArray() }
    };

            DataContext = null;
            DataContext = this;
        }
    }
}