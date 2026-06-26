using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WpfApp1
{
    public partial class DashboardPage : Page
    {
        public ISeries[] TonnageSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }
        public ISeries[] SupplierPieSeries { get; set; }
        public SolidColorPaint LegendPaint { get; set; }

        public DashboardPage()
        {
            InitializeComponent();

            LegendPaint = new SolidColorPaint(SKColors.LightGray);

            TonnageSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 45, 60, 25, 80, 55, 120 },
                    Name = "Тонны",
                    Fill = new SolidColorPaint(SKColors.DarkOrange),
                    MaxBarWidth = 40
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new string[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" },
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            SupplierPieSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 40 }, Name = "ООО Степные Просторы" },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "КФХ Иванов И.И." },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "ЗАО АгроХолдинг-Волга" }
            };

            DataContext = this;
        }
    }
}