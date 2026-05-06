using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Linq;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class AnalyticsPage : Page
    {
        public ISeries[] ChartSeries { get; set; }
        public Axis[] ChartXAxes { get; set; }

        public AnalyticsPage()
        {
            InitializeComponent();

            DataContext = this;

            LoadAnalytics();
        }

        private void LoadAnalytics()
        {
            using var context = new RetailDbContext();

            var totalSales = context.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;
            var totalOrders = context.Orders.Count();
            var totalProducts = context.Products.Count();
            var totalCustomers = context.Customers.Count();

            TxtTotalSales.Text = $"Выручка: {totalSales:F2} ₽";
            TxtTotalOrders.Text = $"Чеков: {totalOrders}";
            TxtTotalProducts.Text = $"Уник. товаров: {totalProducts}";
            TxtTotalCustomers.Text = $"Клиентов: {totalCustomers}";

            var categoriesData = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = c.Products.Count
                })
                .ToList();

            ChartSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = categoriesData.Select(c => c.ProductCount).ToArray(),
                    Name = "Кол-во товаров",
                    MaxBarWidth = 50
                }
            };

            ChartXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = categoriesData.Select(c => c.CategoryName).ToArray(),
                    LabelsRotation = 15
                }
            };
        }
    }
}