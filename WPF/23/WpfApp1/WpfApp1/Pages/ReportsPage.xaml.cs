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
    public partial class ReportsPage : Page
    {
        private AppDbContext _context;

        public ReportsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCharts();
        }

        private void LoadCharts()
        {
            var transactions = _context.FinancialTransactions.ToList();

            var totalIncome = transactions.Where(t => t.Type == "доход").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "расход").Sum(t => t.Amount);

            PieChartInOut.Series = new ISeries[]
            {
                new PieSeries<decimal>
                {
                    Values = new decimal[] { totalIncome },
                    Name = "Доходы",
                    Fill = new SolidColorPaint(SKColors.ForestGreen)
                },
                new PieSeries<decimal>
                {
                    Values = new decimal[] { totalExpense },
                    Name = "Расходы",
                    Fill = new SolidColorPaint(SKColors.Crimson)
                }
            };

            var expensesByCategory = transactions
                .Where(t => t.Type == "расход")
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .ToList();

            BarChartCategory.Series = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Values = expensesByCategory.Select(x => x.Total).ToArray(),
                    Name = "Сумма расходов",
                    Fill = new SolidColorPaint(SKColors.Chocolate)
                }
            };

            BarChartCategory.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = expensesByCategory.Select(x => x.Category).ToArray(),
                    LabelsRotation = 15
                }
            };
        }
    }
}