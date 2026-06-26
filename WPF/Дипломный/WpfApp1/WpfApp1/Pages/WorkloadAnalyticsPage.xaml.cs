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
    public partial class WorkloadAnalyticsPage : Page
    {
        public ISeries[] WorkloadSeries { get; set; }
        public Axis[] YAxes { get; set; }

        public WorkloadAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var db = new AppDbContext();

                int totalCount = db.Equipments.Count(e => e.Status.Name != SystemStatuses.Scrapped);
                int inRepairCount = db.Equipments.Count(e => e.Status.Name == SystemStatuses.InRepair);
                decimal totalValue = db.Equipments
                    .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                    .Sum(e => (decimal?)e.Price ?? 0m);

                txtTotalEquip.Text = $"{totalCount} шт.";
                txtInRepair.Text = $"{inRepairCount} шт.";
                txtTotalValue.Text = $"{totalValue:N2} ₽";

                var stats = db.Equipments
                    .Where(e => e.EmployeeId != null && e.Status.Name != SystemStatuses.Scrapped)
                    .GroupBy(e => e.Employee.FullName)
                    .Select(g => new { Name = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count).Take(10).ToList();

                if (!stats.Any())
                {
                    ChartBorder.Visibility = Visibility.Collapsed;
                    txtNoData.Visibility = Visibility.Visible;
                    return;
                }

                ChartBorder.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;

                WorkloadSeries = new ISeries[]
                {
                    new RowSeries<int>
                    {
                        Values = stats.Select(s => s.Count).ToArray(),
                        Name = "Техника",
                        DataLabelsPaint = new SolidColorPaint(new SKColor(45, 55, 72)),
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
                        DataLabelsFormatter = point => $"{point.Model} шт."
                    }
                };

                YAxes = new Axis[] { new Axis { Labels = stats.Select(s => s.Name).ToArray(), TextSize = 14 } };

                DataContext = null; DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка визуализации:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}