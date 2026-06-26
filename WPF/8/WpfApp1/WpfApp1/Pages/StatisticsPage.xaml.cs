using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class StatisticsPage : Page
    {
        public Func<double, string> Formatter { get; set; }
        public List<string> ClassLabels { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();
            LoadCharts();
        }

        private void LoadCharts()
        {
            using (var db = new CarRentalContext())
            {
                var allCars = db.Автомобили.ToList();
                var allRentals = db.Аренды.Include(a => a.Автомобиль).ToList();

                if (!allCars.Any()) return;

                var classGroups = allCars.GroupBy(c => c.Класс)
                                         .Select(g => new { Name = g.Key, Count = g.Count() });

                SeriesCollection pieSeries = new SeriesCollection();
                foreach (var item in classGroups)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = item.Name,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true
                    });
                }
                ChartClasses.Series = pieSeries;

                var revenueGroups = allRentals.GroupBy(r => r.Автомобиль.Класс)
                                              .Select(g => new { Name = g.Key, Total = g.Sum(r => r.Сумма_Итого ?? 0) })
                                              .ToList();

                ClassLabels = revenueGroups.Select(r => r.Name).ToList();
                Formatter = value => value.ToString("N0") + " ₽";

                ChartRevenue.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Выручка",
                        Values = new ChartValues<decimal>(revenueGroups.Select(r => r.Total)),
                        DataLabels = true,
                        Fill = System.Windows.Media.Brushes.DodgerBlue
                    }
                };

                DataContext = this;
            }
        }
    }
}