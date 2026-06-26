using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class StatisticsPage : Page
    {
        public Func<double, string> Formatter { get; set; }
        public List<string> VehicleLabels { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();
            LoadCharts();
        }

        private void LoadCharts()
        {
            using (var db = new AppDbContext())
            {
                var vehicles = db.Автомобилис.ToList();

                if (!vehicles.Any()) return;

                var statusGroups = vehicles.GroupBy(v => v.Статус ?? "Неизвестно")
                                           .Select(g => new { Status = g.Key, Count = g.Count() });

                SeriesCollection pieSeries = new SeriesCollection();
                foreach (var item in statusGroups)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = item.Status,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true
                    });
                }
                ChartStatuses.Series = pieSeries;

                var mileageData = vehicles.OrderByDescending(v => v.ТекущийПробег ?? 0)
                                          .Take(10)
                                          .ToList();

                VehicleLabels = mileageData.Select(v => v.ГосНомер).ToList();
                Formatter = value => value.ToString("N0") + " км";

                ChartMileage.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Пробег",
                        Values = new ChartValues<int>(mileageData.Select(v => v.ТекущийПробег ?? 0)),
                        DataLabels = true
                    }
                };

                DataContext = this;
            }
        }
    }
}