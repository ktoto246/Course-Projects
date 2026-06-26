using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class FinancialAnalyticsPage : Page
    {
        public ISeries[] FinancialSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public FinancialAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var db = new AppDbContext();

                var stats = db.Equipments
                    .Where(e => e.Status.Name != SystemStatuses.Scrapped)
                    .GroupBy(e => e.Category.Name)
                    .Select(g => new { CategoryName = g.Key, TotalValue = g.Sum(e => (double)(e.Price ?? 0m)) })
                    .ToList();

                FinancialSeries = new ISeries[]
                {
            new ColumnSeries<double>
            {
                Values = stats.Select(s => s.TotalValue).ToArray(),
                Name = "Стоимость фонда",
                XToolTipLabelFormatter = point => $"{point.Context.Series.Name}: {point.Model:N2} ₽"
            }
                };

                XAxes = new Axis[] { new Axis { Labels = stats.Select(s => s.CategoryName).ToArray() } };

                YAxes = new Axis[] { new Axis { MinLimit = 0 } };

                DataContext = null;
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}