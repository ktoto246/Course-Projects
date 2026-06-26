using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RepairCostsAnalyticsPage : Page
    {
        public ISeries[] RepairSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public RepairCostsAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var db = new AppDbContext();

                var repairs = db.RepairHistories
                    .Where(r => r.Cost != null && r.Cost > 0)
                    .ToList()
                    .GroupBy(r => r.DateIn.ToString("MM.yyyy"))
                    .Select(g => new {
                        Month = g.Key,
                        TotalCost = (double)g.Sum(r => r.Cost.Value),
                        RepairCount = g.Count(),
                        Date = DateTime.ParseExact(g.Key, "MM.yyyy", null)
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                if (!repairs.Any()) return;

                RepairSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = repairs.Select(r => r.TotalCost).ToArray(),
                        Name = "Затраты",
                        GeometrySize = 12,
                        DataLabelsFormatter = point => $"{point.Model:N2} руб.",
                        DataLabelsPaint = new SolidColorPaint(new SKColor(45, 55, 72)),
                        DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
                    }
                };

                XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = repairs.Select(r => $"{r.Month}\n({r.RepairCount} шт.)").ToArray(),
                        LabelsRotation = 0
                    }
                };

                YAxes = new Axis[] { new Axis { MinLimit = 0 } };

                DataContext = null;
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}