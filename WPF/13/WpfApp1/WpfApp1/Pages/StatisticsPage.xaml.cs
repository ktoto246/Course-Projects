using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1;

namespace WpfApp1.Pages
{
    public partial class StatisticsPage : Page
    {
        private readonly RegistryContext _context;

        public StatisticsPage()
        {
            InitializeComponent();
            _context = new RegistryContext();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .ToList();

            if (!appointments.Any()) return;

            PieChartSpecializations.Series = appointments
                .GroupBy(a => a.Doctor.Specialization.Name)
                .Select(g => new PieSeries<int>
                {
                    Values = new[] { g.Count() },
                    Name = g.Key
                }).ToArray();

            var doctorData = appointments
                .GroupBy(a => a.Doctor.FullName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            BarChartDoctors.Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = doctorData.Select(d => d.Count).ToArray(),
                    Name = "Приемы"
                }
            };

            BarChartDoctors.XAxes = new[] { new Axis { Labels = doctorData.Select(d => d.Name).ToArray() } };

            var dynamicsData = appointments
                .Where(a => a.AppointmentDate >= DateTime.Now.AddDays(-30))
                .GroupBy(a => a.AppointmentDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("dd.MM"), Count = g.Count() })
                .ToList();

            LineChartDynamics.Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = dynamicsData.Select(d => d.Count).ToArray(),
                    Name = "Записи"
                }
            };

            LineChartDynamics.XAxes = new[] { new Axis { Labels = dynamicsData.Select(d => d.Date).ToArray() } };
        }
    }
}