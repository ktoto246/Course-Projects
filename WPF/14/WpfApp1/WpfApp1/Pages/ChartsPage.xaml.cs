using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ChartsPage : UserControl
    {
        public ChartsPage()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Now.AddYears(-1);
            EndDatePicker.SelectedDate = DateTime.Now;
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCharts();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadCharts();
        }

        private void LoadCharts()
        {
            try
            {
                using var context = new BaltexEquipmentContext();

                DateOnly? start = StartDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value)
                    : null;

                DateOnly? end = EndDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                    : null;

                LoadCostLineChart(context, start, end);
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка загрузки графика: " + ex.Message;
            }
        }

        private void LoadCostLineChart(BaltexEquipmentContext context, DateOnly? startDate, DateOnly? endDate)
        {
            var query = context.MaintenanceRecords.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.MaintenanceDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.MaintenanceDate <= endDate.Value);

            var rawGroups = query
                .GroupBy(m => new { m.MaintenanceDate.Year, m.MaintenanceDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalCost = g.Sum(x => x.Cost)
                })
                .ToList();

            var data = rawGroups
                .Select(x => new {
                    Month = new DateOnly(x.Year, x.Month, 1),
                    TotalCost = x.TotalCost
                })
                .OrderBy(x => x.Month)
                .ToList();

            var series = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = data.Select(x => x.TotalCost).ToArray(),
                    Name = "Затраты (₽)",
                    Fill = new SolidColorPaint(new SKColor(231, 76, 60)),
                    Stroke = new SolidColorPaint(new SKColor(231, 76, 60), 2),
                    GeometrySize = 8
                }
            };

            CostLineChart.Series = series;
            CostLineChart.XAxes = new[]
            {
                new Axis
                {
                    Labels = data.Select(x => x.Month.ToString("MMM yyyy")).ToArray()
                }
            };
            CostLineChart.YAxes = new[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} ₽"
                }
            };
        }
    }
}