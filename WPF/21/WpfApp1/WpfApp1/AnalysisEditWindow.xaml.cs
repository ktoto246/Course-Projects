using System;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public partial class AnalysisEditWindow : Window
    {
        public AnalysisLog CurrentAnalysis { get; private set; }

        public AnalysisEditWindow(AnalysisLog analysis, BalashovDbContext context)
        {
            InitializeComponent();
            CurrentAnalysis = analysis;

            BatchComboBox.ItemsSource = context.GrainBatches.ToList();
            StaffComboBox.ItemsSource = context.LaboratoryStaff.ToList();
            IndicatorComboBox.ItemsSource = context.QualityIndicators.ToList();

            if (CurrentAnalysis.AnalysisId != 0)
            {
                BatchComboBox.SelectedValue = CurrentAnalysis.BatchId;
                StaffComboBox.SelectedValue = CurrentAnalysis.StaffId;
                IndicatorComboBox.SelectedValue = CurrentAnalysis.IndicatorId;
            }

            ValueBox.Text = CurrentAnalysis.AnalysisValue == 0 ? "" : CurrentAnalysis.AnalysisValue.ToString();
            DateBox.Text = CurrentAnalysis.AnalysisDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (BatchComboBox.SelectedValue == null || StaffComboBox.SelectedValue == null || IndicatorComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все выпадающие списки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(ValueBox.Text.Replace(".", ","), out decimal parsedValue))
            {
                MessageBox.Show("Неверный формат результата!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParse(DateBox.Text, out DateTime parsedDate))
            {
                MessageBox.Show("Неверный формат даты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentAnalysis.BatchId = (int)BatchComboBox.SelectedValue;
            CurrentAnalysis.StaffId = (int)StaffComboBox.SelectedValue;
            CurrentAnalysis.IndicatorId = (int)IndicatorComboBox.SelectedValue;
            CurrentAnalysis.AnalysisValue = parsedValue;
            CurrentAnalysis.AnalysisDate = parsedDate;

            DialogResult = true;
        }
    }
}