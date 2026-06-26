using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Data;

namespace WpfApp1.Pages
{
    public partial class AdminDashboardPage : Page
    {
        private AppDbContext _db;

        public AdminDashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var today = DateTime.Today;

                int totalEmp = _db.Employees.Count();
                int activeEmp = _db.Employees.Count(e => e.IsActive);

                int batchesToday = _db.GrainBatches.Count(b => b.ArrivalDate.Date == today);

                var storages = _db.Storages.ToList();
                int fullStoragesCount = storages.Count(s => s.Capacity > 0 && (s.CurrentLoad / s.Capacity) >= 0.8m);

                txtTotalEmployees.Text = totalEmp.ToString();
                txtActiveEmployees.Text = activeEmp.ToString();
                txtBatchesToday.Text = batchesToday.ToString();
                txtFullStorages.Text = fullStoragesCount.ToString();

                var storageData = storages.Select(s =>
                {
                    double percent = s.Capacity > 0 ? (double)(s.CurrentLoad / s.Capacity * 100) : 0;
                    string color = percent >= 80 ? "#EF4444" : (percent >= 50 ? "#F59E0B" : "#10B981");
                    return new StorageStat
                    {
                        Name = s.Name,
                        Percentage = percent,
                        PercentageText = $"{Math.Round(percent)}%",
                        BarColor = (SolidColorBrush)new BrushConverter().ConvertFrom(color)
                    };
                }).ToList();

                icStorages.ItemsSource = storageData;

                var roleGroups = _db.Employees
                    .GroupBy(e => e.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToList();

                SeriesCollection pieSeries = new SeriesCollection();
                foreach (var rg in roleGroups)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = rg.Role,
                        Values = new ChartValues<double> { (double)rg.Count },
                        DataLabels = true
                    });
                }
                pieChartRoles.Series = pieSeries;

                var last7Days = Enumerable.Range(0, 7)
                    .Select(i => today.AddDays(-i))
                    .Reverse()
                    .ToList();

                var batchStats = _db.GrainBatches
                    .Where(b => b.ArrivalDate >= last7Days.First())
                    .ToList()
                    .GroupBy(b => b.ArrivalDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var lineValues = new ChartValues<double>();
                var labels = new List<string>();

                foreach (var day in last7Days)
                {
                    labels.Add(day.ToString("dd.MM"));
                    lineValues.Add((double)(batchStats.ContainsKey(day) ? batchStats[day] : 0));
                }

                lineChartBatches.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Партии",
                        Values = lineValues,
                        PointGeometrySize = 10,
                        Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#3B82F6"),
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#93C5FD"),
                        Opacity = 0.4
                    }
                };
                axisXDays.Labels = labels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class StorageStat
        {
            public string? Name { get; set; }
            public double Percentage { get; set; }
            public string? PercentageText { get; set; }
            public SolidColorBrush? BarColor { get; set; }
        }
    }
}