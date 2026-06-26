using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class StatisticsPage : Page
    {
        private BdContext db;

        public StatisticsPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadCharts();
        }

        private void LoadCharts()
        {
            var fleetData = db.Плавсредстваs
                .Include(p => p.IdКатегорииNavigation)
                .GroupBy(p => p.IdКатегорииNavigation.Название)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            PieChartFleet.Series = fleetData.Select(d => new PieSeries<int>
            {
                Values = new int[] { d.Count },
                Name = d.Name
            }).ToArray();

            var revenueData = db.Арендаs
                .Include(a => a.IdПлавсредстваNavigation)
                    .ThenInclude(p => p.IdКатегорииNavigation)
                .Where(a => a.ИтогоКОплате != null)
                .ToList() 
                .GroupBy(a => a.IdПлавсредстваNavigation.IdКатегорииNavigation.Название)
                .Select(g => new { Name = g.Key, Total = (double)g.Sum(a => a.ИтогоКОплате ?? 0) })
                .ToList();

            BarChartRevenue.Series = revenueData.Select(d => new ColumnSeries<double>
            {
                Values = new double[] { d.Total },
                Name = d.Name
            }).ToArray();

            BarChartRevenue.XAxes = new Axis[]
            {
                new Axis { Name = "Категории" }
            };
        }
    }
}