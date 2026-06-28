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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : Page
    {
        private Service? _selectedService;
        public ServicesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new BmtcContext())
            {
                ServicesGrid.ItemsSource = db.Services.ToList();
            }
        }

        private void ServicesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServicesGrid.SelectedItem is Service service)
            {
                _selectedService = service;
                TxtName.Text = service.Name;
                TxtPrice.Text = service.Price.ToString();
            }
        }

        private bool ValidateInput(out decimal parsedPrice)
        {
            parsedPrice = 0;

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Название услуги не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text.Replace(".", ","), out parsedPrice) || parsedPrice < 0)
            {
                MessageBox.Show("Введите корректную цену (число больше или равное нулю)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out decimal price)) return;

            using (var db = new BmtcContext())
            {
                var newService = new Service
                {
                    Name = TxtName.Text.Trim(),
                    Price = price
                };

                db.Services.Add(newService);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedService == null)
            {
                MessageBox.Show("Выберите услугу для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(out decimal price)) return;

            using (var db = new BmtcContext())
            {
                var serviceToUpdate = db.Services.FirstOrDefault(x => x.Id == _selectedService.Id);
                if (serviceToUpdate != null)
                {
                    serviceToUpdate.Name = TxtName.Text.Trim();
                    serviceToUpdate.Price = price;
                    db.SaveChanges();
                }
            }
            LoadData();
            ClearForm();
        }

        private void BtnDelete_Click(object senser, RoutedEventArgs e)
        {
            if (_selectedService == null)
            {
                MessageBox.Show("Выберите услугу для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить услугу '{_selectedService.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new BmtcContext())
                {
                    var serviceToDelete = db.Services.FirstOrDefault(x => x.Id == _selectedService.Id);
                    if (serviceToDelete != null)
                    {
                        db.Services.Remove(serviceToDelete);
                        db.SaveChanges();
                    }
                }

                LoadData();
                ClearForm();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedService = null;
            ServicesGrid.SelectedItem = null;
            TxtName.Clear();
            TxtPrice.Clear();
        }
    }
}
