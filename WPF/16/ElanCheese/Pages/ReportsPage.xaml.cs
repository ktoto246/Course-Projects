using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class ReportsPage : Page
    {
        private ElanCheeseDbContext _context;

        public ReportsPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadReports();
        }

        private void LoadReports()
        {
            // Pie Chart - Считаем реальный вес сыра, а не просто количество партий
            var cheeseWeights = _context.CheeseBatches
                .Include(b => b.Variety)
                .GroupBy(b => b.Variety.Name)
                .Select(g => new { Name = g.Key, TotalWeight = (double)g.Sum(b => b.WeightKg) })
                .ToList();

            var pieSeries = new SeriesCollection();
            foreach (var item in cheeseWeights)
            {
                pieSeries.Add(new PieSeries
                {
                    Values = new ChartValues<double> { item.TotalWeight },
                    Title = item.Name,
                    DataLabels = true,
                    // Форматируем текст на дольках: Вес + Процент
                    LabelPoint = chartPoint => string.Format("{0:N0} кг ({1:P0})", chartPoint.Y, chartPoint.Participation)
                });
            }

            PieChart.Series = pieSeries;

            // Cartesian Chart - Переделываем в гистограмму (столбцы)
            var monthlyProduction = _context.CheeseBatches
                .GroupBy(b => new { b.ProductionDate.Year, b.ProductionDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalWeight = (double)g.Sum(b => b.WeightKg) })
                .ToList();

            var monthNames = monthlyProduction.Select(m => $"{m.Month:D2}/{m.Year}").ToArray();
            var weights = monthlyProduction.Select(m => m.TotalWeight).ToArray();

            var cartesianSeries = new SeriesCollection
            {
                new ColumnSeries // Поменяли LineSeries на ColumnSeries
                {
                    Title = "Произведено (кг)",
                    Values = new ChartValues<double>(weights),
                    Fill = (Brush)new BrushConverter().ConvertFrom("#F59E0B"), // Красим столбцы в наш акцентный цвет
                    MaxColumnWidth = 40
                }
            };

            CartesianChart.Series = cartesianSeries;
            CartesianChart.AxisX.Add(new Axis
            {
                Labels = monthNames,
                LabelsRotation = 15, // Сделали наклон текста чуть аккуратнее
                Separator = new LiveCharts.Wpf.Separator { Step = 1 }
            });
            CartesianChart.AxisY.Add(new Axis
            {
                Title = "Вес (кг)",
                LabelFormatter = value => value.ToString("N0") + " кг" // Добавили "кг" к оси Y
            });
        }
    }
}