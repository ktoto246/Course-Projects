using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DashboardPage : Page
    {
        private AppDbContext _context;

        public DashboardPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadChartData();
        }

        private void LoadChartData()
        {
            var chartData = _context.Mills
                .Select(m => new
                {
                    MillName = m.MillName,
                    TotalQuantity = m.Orders
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => (decimal?)od.QuantityTons) ?? 0
                })
                .Where(x => x.TotalQuantity > 0)
                .OrderBy(x => x.MillName)
                .ToList();

            var values = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var item in chartData)
            {
                values.Add((double)item.TotalQuantity);
                labels.Add(item.MillName);
            }

            var series = new ColumnSeries
            {
                Values = values,
                Title = "Тонны",
                DataLabels = true,
                Fill = (Brush)new BrushConverter().ConvertFrom("#DEB887")
            };

            chart.Series.Clear();
            chart.Series.Add(series);

            axisX.Labels = labels;
        }
    }
}