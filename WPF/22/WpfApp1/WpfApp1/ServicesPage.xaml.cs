using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class ServicesPage : Page
    {
        private AlphabetContext _context;

        public ServicesPage()
        {
            InitializeComponent();
            _context = new AlphabetContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Services.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(s => s.ServiceName.ToLower().Contains(searchText));
            }

            ServicesGrid.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        public void AddService()
        {
            var newService = new Service();
            var window = new ServiceWindow(newService);

            if (window.ShowDialog() == true)
            {
                _context.Services.Add(newService);
                _context.SaveChanges();
                LoadData(TxtSearch.Text);
            }
        }

        public void EditService()
        {
            if (ServicesGrid.SelectedItem is Service selectedService)
            {
                var window = new ServiceWindow(selectedService);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadData(TxtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void DeleteService()
        {
            if (ServicesGrid.SelectedItem is Service selectedService)
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить услугу {selectedService.ServiceName}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Services.Remove(selectedService);
                        _context.SaveChanges();
                        LoadData(TxtSearch.Text);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Не удалось удалить услугу. Возможно, она используется в заказах.\nОшибка: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}