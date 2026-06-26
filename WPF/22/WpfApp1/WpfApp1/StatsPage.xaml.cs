using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class StatsPage : Page
    {
        private AlphabetContext _context;

        public StatsPage()
        {
            InitializeComponent();
            _context = new AlphabetContext();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var orderDetails = _context.OrderDetails
                .Include(od => od.Service)
                .ToList();

            var revenueByService = orderDetails
                .Where(od => od.Service != null)
                .GroupBy(od => od.Service.ServiceName)
                .Select(g => new
                {
                    ServiceName = g.Key,
                    TotalRevenue = g.Sum(od => od.Quantity * od.Service.BasePrice)
                })
                .ToList();

            SeriesCollection pieSeries = new SeriesCollection();
            foreach (var item in revenueByService)
            {
                pieSeries.Add(new PieSeries
                {
                    Title = item.ServiceName,
                    Values = new ChartValues<decimal> { item.TotalRevenue },
                    DataLabels = true
                });
            }
            RevenuePieChart.Series = pieSeries;

            var orders = _context.Orders
                .Include(o => o.Employee)
                .ToList();

            var ordersByEmployee = orders
                .Where(o => o.Employee != null)
                .GroupBy(o => o.Employee.FullName)
                .Select(g => new
                {
                    EmployeeName = g.Key,
                    OrderCount = g.Count()
                })
                .ToList();

            SeriesCollection columnSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Заказы",
                    Values = new ChartValues<int>(ordersByEmployee.Select(x => x.OrderCount))
                }
            };

            EmployeeOrdersChart.Series = columnSeries;
            AxisEmployeeNames.Labels = ordersByEmployee.Select(x => x.EmployeeName).ToArray();
        }
    }
}