using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;

namespace WpfApp1.Pages
{
    public partial class ManagerDashboardPage : Page
    {
        private AppDbContext _db;
        public Func<double, string> CurrencyFormatter { get; set; }

        public ManagerDashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();

            CurrencyFormatter = value => value.ToString("N0") + " ₽";
            DataContext = this;

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var allServices = _db.RenderedServices
                    .Include(rs => rs.Service)
                    .Include(rs => rs.Batch).ThenInclude(b => b.Client)
                    .Where(rs => rs.RecordDate >= startOfMonth.AddMonths(-12))
                    .ToList();

                var todayServices = allServices.Where(rs => rs.RecordDate.Date == today).ToList();
                var monthServices = allServices.Where(rs => rs.RecordDate.Date >= startOfMonth && rs.RecordDate.Date <= today).ToList();

                decimal revToday = todayServices.Sum(rs => rs.TotalPrice);
                txtRevenueToday.Text = $"{revToday:N0} ₽";
                txtCountToday.Text = $"{todayServices.Count} услуг";

                decimal revMonth = monthServices.Sum(rs => rs.TotalPrice);
                txtRevenueMonth.Text = $"{revMonth:N0} ₽";
                txtCountMonth.Text = $"{monthServices.Count} услуг";

                var batchesWithLab = _db.GrainBatches.Where(b => b.LabTest != null).Select(b => b.Id).ToList();
                var batchesWithServices = _db.RenderedServices.Select(rs => rs.BatchId).Distinct().ToList();
                int noServicesCount = batchesWithLab.Except(batchesWithServices).Count();
                txtNoServices.Text = noServicesCount.ToString();

                int uniqueBatchesThisMonth = monthServices.Select(rs => rs.BatchId).Distinct().Count();
                decimal avgCheck = uniqueBatchesThisMonth > 0 ? revMonth / uniqueBatchesThisMonth : 0;
                txtAvgCheck.Text = $"{avgCheck:N0} ₽";

                var pieData = allServices
                    .Where(rs => rs.Service != null)
                    .GroupBy(rs => rs.Service.Name)
                    .Select(g => new { Name = g.Key, Total = g.Sum(x => x.TotalPrice) })
                    .Where(x => x.Total > 0)
                    .ToList();

                SeriesCollection pieSeries = new SeriesCollection();
                foreach (var item in pieData)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = item.Name,
                        Values = new ChartValues<double> { (double)item.Total },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0} ₽"
                    });
                }
                pieChartServices.Series = pieSeries;

                var clientData = allServices
                    .Where(rs => rs.Batch?.Client != null)
                    .GroupBy(rs => rs.Batch.Client.CompanyName)
                    .Select(g => new ClientStat
                    {
                        ClientName = g.Key,
                        Revenue = (double)g.Sum(x => x.TotalPrice),
                        RevenueText = $"{g.Sum(x => x.TotalPrice) / 1000:N0}k ₽"
                    })
                    .OrderByDescending(c => c.Revenue)
                    .Take(4)
                    .ToList();

                double maxRev = clientData.Any() ? clientData.Max(c => c.Revenue) : 1;
                foreach (var c in clientData) c.MaxRevenue = maxRev;

                icTopClients.ItemsSource = clientData;

                int daysInMonth = Math.Max(7, (today - startOfMonth).Days + 1);
                var dailyRevenue = monthServices
                    .GroupBy(rs => rs.RecordDate.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(rs => rs.TotalPrice));

                var lineValues = new ChartValues<double>();
                var labels = new List<string>();

                for (int i = 0; i < daysInMonth; i++)
                {
                    var day = startOfMonth.AddDays(i);
                    labels.Add(day.ToString("dd"));
                    lineValues.Add((double)(dailyRevenue.ContainsKey(day) ? dailyRevenue[day] : 0));
                }

                lineChartRevenue.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Выручка",
                        Values = lineValues,
                        PointGeometrySize = 8,
                        Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#8B5CF6"),
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#C4B5FD"),
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

        public class ClientStat
        {
            public string? ClientName { get; set; }
            public double Revenue { get; set; }
            public double MaxRevenue { get; set; }
            public string? RevenueText { get; set; }
        }
    }
}