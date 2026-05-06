using System.Linq;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;

namespace WpfApp1
{
    public partial class AnalyticsPage : Page
    {
        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }

        public AnalyticsPage()
        {
            InitializeComponent();
            LoadChartData();
            DataContext = this;
        }

        private void LoadChartData()
        {
            using var context = new AppDbContext();

            var deals = context.Deals.Include(d => d.Product).ToList();

            var data = deals
                .GroupBy(d => d.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    Total = (double)g.Sum(d => d.TotalAmount)
                })
                .ToList();

            Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = data.Select(d => d.Total).ToArray(),
                    Name = "Выручка (руб.)"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = data.Select(d => d.ProductName).ToArray()
                }
            };
        }
    }
}