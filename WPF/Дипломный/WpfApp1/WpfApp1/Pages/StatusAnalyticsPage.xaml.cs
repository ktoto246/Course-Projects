using System;
using System.Collections.Generic;
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
            try
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
                        DataLabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{stat.StatusName}: {point.Model} шт.",

                        HoverPushout = 0
                    });
                }

                StatusSeries = series.ToArray();

                DataContext = null;
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}