using System.Windows;

namespace WpfApp1
{
    public partial class IndicatorEditWindow : Window
    {
        public QualityIndicator CurrentIndicator { get; private set; }

        public IndicatorEditWindow(QualityIndicator indicator)
        {
            InitializeComponent();
            CurrentIndicator = indicator;
            NameBox.Text = CurrentIndicator.Name;
            UnitBox.Text = CurrentIndicator.Unit;
            MinBox.Text = CurrentIndicator.MinNormalValue == 0 ? "" : CurrentIndicator.MinNormalValue.ToString();
            MaxBox.Text = CurrentIndicator.MaxNormalValue == 0 ? "" : CurrentIndicator.MaxNormalValue.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(UnitBox.Text))
            {
                MessageBox.Show("Наименование и единица измерения обязательны!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(MinBox.Text.Replace(".", ","), out decimal parsedMin))
            {
                MessageBox.Show("Неверный формат минимальной нормы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(MaxBox.Text.Replace(".", ","), out decimal parsedMax))
            {
                MessageBox.Show("Неверный формат максимальной нормы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentIndicator.Name = NameBox.Text;
            CurrentIndicator.Unit = UnitBox.Text;
            CurrentIndicator.MinNormalValue = parsedMin;
            CurrentIndicator.MaxNormalValue = parsedMax;

            DialogResult = true;
        }
    }
}