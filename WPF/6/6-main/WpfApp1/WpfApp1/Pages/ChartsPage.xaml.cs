using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ChartsPage : Page
    {
        private readonly ApplicationDbContext _context;

        public SeriesCollection VolumeSeriesCollection { get; set; }
        public List<string> ProductLabels { get; set; }
        public SeriesCollection RevenueSeriesCollection { get; set; }

        public ChartsPage()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            
            LoadChartsData();
            
            DataContext = this;
        }

        private void LoadChartsData()
        {
            var salesData = _context.Реализации
                .Include(r => r.Нефтепродукт)
                .ToList()
                .GroupBy(r => r.Нефтепродукт.Название)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalVolume = g.Sum(r => r.Количество_Тонн),
                    TotalRevenue = g.Sum(r => r.Общая_Стоимость)
                })
                .ToList();

            // Setup Column Chart
            VolumeSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Объем",
                    Values = new ChartValues<decimal>(salesData.Select(d => d.TotalVolume)),
                    DataLabels = true
                }
            };

            ProductLabels = salesData.Select(d => d.ProductName).ToList();
            SalesVolumeChart.Series = VolumeSeriesCollection;

            // Setup Pie Chart
            RevenueSeriesCollection = new SeriesCollection();
            foreach (var data in salesData)
            {
                RevenueSeriesCollection.Add(new PieSeries
                {
                    Title = data.ProductName,
                    Values = new ChartValues<decimal> { data.TotalRevenue },
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
                });
            }

            RevenuePieChart.Series = RevenueSeriesCollection;
        }
    }
}
