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
    public partial class CategoryAnalyticsPage : Page
    {
        public ISeries[] CategorySeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public CategoryAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var db = new AppDbContext();

                var stats = db.Equipments
                    .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                    .GroupBy(e => e.Category.Name)
                    .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                    .ToList();

                if (!stats.Any()) return;

                CategorySeries = new ISeries[]
                {
                    new ColumnSeries<int>
                    {
                        Values = stats.Select(s => s.Count).ToArray(),
                        Name = "Количество",
                        DataLabelsFormatter = point => $"{point.Model} шт.",
                        DataLabelsPaint = new SolidColorPaint(new SKColor(45, 55, 72)),
                        DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
                    }
                };

                XAxes = new Axis[] { new Axis { Labels = stats.Select(s => s.CategoryName).ToArray(), LabelsRotation = 15 } };

                YAxes = new Axis[] { new Axis { MinLimit = 0, MinStep = 1 } };

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