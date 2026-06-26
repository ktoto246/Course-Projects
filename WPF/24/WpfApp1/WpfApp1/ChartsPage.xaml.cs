using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class ChartsPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }

        public ChartsPage()
        {
            InitializeComponent();
            LoadChartData();
            DataContext = this;
        }

        private void LoadChartData()
        {
            var deliveryData = _db.Deliveries
                .Include(d => d.Product)
                .GroupBy(d => d.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(d => d.Quantity)
                })
                .ToList();

            var quantities = deliveryData.Select(d => (double)d.TotalQuantity).ToArray();
            var productNames = deliveryData.Select(d => d.ProductName).ToArray();

            Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = quantities,
                    Name = "Принято (шт/кг)",
                    Fill = new SolidColorPaint(new SkiaSharp.SKColor(227, 6, 17))
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = productNames,
                    LabelsRotation = 15,
                    TextSize = 14
                }
            };
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChartData();

            MainChart.Series = Series;
            MainChart.XAxes = XAxes;
        }
    }
}