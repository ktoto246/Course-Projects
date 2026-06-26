using System.Windows;

namespace WpfApp1
{
    public partial class StaffEditWindow : Window
    {
        public LaboratoryStaff CurrentStaff { get; private set; }

        public StaffEditWindow(LaboratoryStaff staff)
        {
            InitializeComponent();
            CurrentStaff = staff;
            NameBox.Text = CurrentStaff.FullName;
            PositionBox.Text = CurrentStaff.Position;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PositionBox.Text))
            {
                MessageBox.Show("ФИО и должность обязательны для заполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentStaff.FullName = NameBox.Text;
            CurrentStaff.Position = PositionBox.Text;

            DialogResult = true;
        }
    }
}