using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Helpers;
using LiveCharts;
using LiveCharts.Wpf;

namespace WpfApp1.Pages
{
    public partial class LabDashboardPage : Page
    {
        private AppDbContext _db;

        public LabDashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (CurrentSession.CurrentUser == null) return;
                int currentUserId = CurrentSession.CurrentUser.Id;

                var today = DateTime.Today;

                txtWaiting.Text = _db.GrainBatches.Count(b => b.Status == "Ожидает анализа").ToString();

                var todayTests = _db.LabTests.Include(t => t.Batch).ThenInclude(b => b.Crop)
                    .Where(t => t.TestDate.Date == today).ToList();

                txtDoneToday.Text = todayTests.Count.ToString();

                int highMoisture = todayTests.Count(t => t.Batch.Crop.BaseMoisture > 0 && t.Moisture > t.Batch.Crop.BaseMoisture);
                txtHighMoisture.Text = highMoisture.ToString();

                decimal avgM = todayTests.Any() ? todayTests.Average(t => t.Moisture) : 0;
                txtAvgMoisture.Text = $"{avgM:F1}%";

                var moistureStats = todayTests
                    .GroupBy(t => t.Batch.Crop.Name)
                    .Select(g => {
                        decimal avg = g.Average(x => x.Moisture);
                        decimal norm = g.First().Batch.Crop.BaseMoisture;
                        bool isHigh = norm > 0 && avg > norm;
                        return new MoistureNormStat
                        {
                            CropName = g.Key,
                            CurrentValue = (double)avg,
                            ValueText = $"{avg:F1}%",
                            NormText = $"Норма: {norm:F1}%",
                            BarColor = isHigh ? Brushes.Crimson : Brushes.DodgerBlue,
                            TextColor = isHigh ? Brushes.Crimson : Brushes.DimGray
                        };
                    }).ToList();
                icMoistureNorms.ItemsSource = moistureStats;

                var weedStats = todayTests
                    .OrderByDescending(t => t.Weediness - t.Batch.Crop.BaseWeediness)
                    .Take(4)
                    .Select(t => new WeedStat
                    {
                        BatchInfo = $"Партия #{t.BatchId} ({t.Batch.CarNumber})",
                        Value = (double)t.Weediness,
                        ValueText = $"{t.Weediness:F1}%"
                    }).ToList();
                icWeedinessStats.ItemsSource = weedStats;

                var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).Reverse().ToList();
                var myTests = _db.LabTests
                    .Where(t => t.LabTechId == currentUserId && t.TestDate >= last7Days.First())
                    .ToList()
                    .GroupBy(t => t.TestDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var values = new ChartValues<double>();
                var labels = new List<string>();

                foreach (var day in last7Days)
                {
                    labels.Add(day.ToString("dd.MM"));
                    values.Add((double)(myTests.ContainsKey(day) ? myTests[day] : 0));
                }

                chartProductivity.Series = new SeriesCollection {
                    new LineSeries {
                        Title = "Мои анализы",
                        Values = values,
                        Stroke = Brushes.MediumSeaGreen,
                        Fill = new SolidColorBrush(Color.FromArgb(30, 60, 179, 113))
                    }
                };
                axisX.Labels = labels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class MoistureNormStat
        {
            public string? CropName { get; set; }
            public double CurrentValue { get; set; }
            public string? ValueText { get; set; }
            public string? NormText { get; set; }
            public SolidColorBrush? BarColor { get; set; }
            public SolidColorBrush? TextColor { get; set; }
        }

        public class WeedStat
        {
            public string? BatchInfo { get; set; }
            public double Value { get; set; }
            public string? ValueText { get; set; }
        }
    }
}