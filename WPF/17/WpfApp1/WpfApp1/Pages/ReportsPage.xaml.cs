using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            dpFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dpTo.SelectedDate = DateTime.Now;
            LoadReports();
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (dpFrom.SelectedDate == null || dpTo.SelectedDate == null)
            {
                MessageBox.Show("Укажите период для отчета.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpFrom.SelectedDate > dpTo.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadReports();
        }

        private void LoadReports()
        {
            using var context = new PrikhoperskoeDbContext();

            var fromDate = dpFrom.SelectedDate.Value.Date;
            var toDate = dpTo.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);

            var incidents = context.Incidents
                .Include(i => i.District)
                .Include(i => i.EquipmentType)
                .Where(i => i.CreatedAt >= fromDate && i.CreatedAt <= toDate)
                .ToList();

            LoadDistrictsChart(incidents);
            LoadEquipmentChart(incidents);
            LoadStatusChart(incidents);
            LoadDynamicsChart(incidents, fromDate, toDate);
        }

        private void LoadDistrictsChart(List<Incident> incidents)
        {
            var data = incidents
                .GroupBy(i => i.District.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            chartDistricts.Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = data.Select(d => d.Count).ToArray(),
                    Name = "Инциденты",
                    Fill = new SolidColorPaint(SKColors.SteelBlue),
                    MaxBarWidth = 50
                }
            };

            chartDistricts.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = data.Select(d => d.Name).ToArray(),
                    LabelsRotation = 0
                }
            };

            chartDistricts.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Количество",
                    MinLimit = 0,
                    MinStep = 1
                }
            };
        }

        private void LoadEquipmentChart(List<Incident> incidents)
        {
            var data = incidents
                .GroupBy(i => i.EquipmentType.Name)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToList();

            var series = data.Select(d => new PieSeries<int>
            {
                Values = new int[] { d.Value },
                Name = d.Name,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{d.Name}\n{point.Model} ({point.StackedValue?.Share:P1})"
            }).ToArray();

            chartEquipment.Series = series;
        }

        private void LoadStatusChart(List<Incident> incidents)
        {
            var data = incidents
                .GroupBy(i => i.Status)
                .Select(g => new { Name = g.Key, Value = g.Count() })
                .ToList();

            var series = data.Select(d => new PieSeries<int>
            {
                Values = new int[] { d.Value },
                Name = d.Name,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsSize = 14,
                DataLabelsFormatter = point => $"{d.Name}\n{point.Model} ({point.StackedValue?.Share:P1})"
            }).ToArray();

            chartStatus.Series = series;
        }

        private void LoadDynamicsChart(List<Incident> incidents, DateTime fromDate, DateTime toDate)
        {
            var daysCount = (toDate.Date - fromDate.Date).Days + 1;
            var dailyData = new int[daysCount];
            var labels = new string[daysCount];

            for (int i = 0; i < daysCount; i++)
            {
                var date = fromDate.AddDays(i);
                labels[i] = date.ToString("dd.MM");
                dailyData[i] = incidents.Count(inc => inc.CreatedAt.Date == date);
            }

            chartDynamics.Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = dailyData,
                    Name = "Инциденты",
                    GeometrySize = 8,
                    Fill = new SolidColorPaint(SKColors.Transparent),
                    Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 3 },
                    GeometryStroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 2 }
                }
            };

            chartDynamics.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = 15
                }
            };

            chartDynamics.YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Количество",
                    MinLimit = 0,
                    MinStep = 1
                }
            };
        }
    }
}