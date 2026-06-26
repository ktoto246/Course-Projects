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

            var today = DateTime.Today;
            dpStart.SelectedDate = new DateTime(today.Year, today.Month, 1);
            dpEnd.SelectedDate = today;

            LoadDashboardData();
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Укажите начальную и конечную даты для формирования отчета.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = dpStart.SelectedDate.Value;
            DateTime endDate = dpEnd.SelectedDate.Value;

            if (startDate > endDate)
            {
                MessageBox.Show("Начальная дата не может быть позже конечной!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);

                dpStart.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dpEnd.SelectedDate = DateTime.Today;
                return;
            }

            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            try
            {

                var filteredServices = _db.RenderedServices
                    .Include(rs => rs.Service)
                    .Include(rs => rs.Batch).ThenInclude(b => b.Client)
                    .Where(rs => rs.RecordDate >= startDate && rs.RecordDate <= endDate)
                    .ToList();

                decimal totalRevenue = filteredServices.Sum(rs => rs.TotalPrice);
                txtRevenueToday.Text = totalRevenue.ToString("N0") + " ₽";
                txtServicesCount.Text = filteredServices.Count.ToString() + " ед.";

                int pendingCount = _db.GrainBatches
                    .Where(b => _db.LabTests.Any(t => t.BatchId == b.Id) && !_db.RenderedServices.Any(rs => rs.BatchId == b.Id))
                    .Count();
                txtPendingBatches.Text = pendingCount.ToString() + " шт.";

                var uniqueBatchesCount = filteredServices.Select(rs => rs.BatchId).Distinct().Count();
                decimal avgCheck = uniqueBatchesCount > 0 ? totalRevenue / uniqueBatchesCount : 0;
                txtAverageCheck.Text = avgCheck.ToString("N0") + " ₽";

                var revenueByService = filteredServices
                    .GroupBy(rs => rs.Service.Name)
                    .Select(g => new { ServiceName = g.Key, Sum = (double)g.Sum(rs => rs.TotalPrice) })
                    .ToList();

                var pieSeries = new SeriesCollection();
                foreach (var item in revenueByService)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = item.ServiceName,
                        Values = new ChartValues<double> { item.Sum },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y:N0} ₽ ({chartPoint.Participation:P0})"
                    });
                }
                pieChartRevenue.Series = pieSeries;

                var clientStats = filteredServices
                    .GroupBy(rs => rs.Batch.Client.CompanyName)
                    .Select(g => new { ClientName = g.Key, Revenue = (double)g.Sum(rs => rs.TotalPrice) })
                    .OrderByDescending(x => x.Revenue)
                    .Take(5)
                    .ToList();

                double maxRevenue = clientStats.Any() ? clientStats.Max(x => x.Revenue) : 100;
                var displayClients = clientStats.Select(c => new ClientStat
                {
                    ClientName = c.ClientName,
                    Revenue = c.Revenue,
                    MaxRevenue = maxRevenue,
                    RevenueText = c.Revenue.ToString("N0") + " ₽"
                }).ToList();
                icTopClients.ItemsSource = displayClients;

                var dailyRevenue = filteredServices
                    .GroupBy(rs => rs.RecordDate.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(rs => rs.TotalPrice));

                var lineValues = new ChartValues<double>();
                var xLabels = new List<string>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    xLabels.Add(date.ToString("dd.MM"));
                    decimal rev = dailyRevenue.ContainsKey(date) ? dailyRevenue[date] : 0;
                    lineValues.Add((double)rev);
                }

                lineChartRevenue.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Выручка",
                        Values = lineValues,
                        PointGeometrySize = 6,
                        Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#8B5CF6"),
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#E9E3FF"),
                        Opacity = 0.4
                    }
                };
                axisXDays.Labels = xLabels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации financial отчета:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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