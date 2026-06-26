using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Client _client;

        public ClientWindow(Client client)
        {
            InitializeComponent();
            _client = client;
            TxtCompany.Text = _client.CompanyName;
            TxtContact.Text = _client.ContactName;
            TxtPhone.Text = _client.Phone;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtContact.Text) || string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show("Имя и Телефон обязательны для заполнения!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client.CompanyName = string.IsNullOrWhiteSpace(TxtCompany.Text) ? null : TxtCompany.Text;
            _client.ContactName = TxtContact.Text;
            _client.Phone = TxtPhone.Text;

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
