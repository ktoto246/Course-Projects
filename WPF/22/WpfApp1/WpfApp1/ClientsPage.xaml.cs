using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1;

namespace WpfApp1
{
    public partial class ClientsPage : Page
    {
        private AlphabetContext _context;

        public ClientsPage()
        {
            InitializeComponent();
            _context = new AlphabetContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(c =>
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(searchText)) ||
                    c.ContactName.ToLower().Contains(searchText) ||
                    c.Phone.Contains(searchText));
            }

            ClientsGrid.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        public void AddClient()
        {
            var newClient = new Client();
            var window = new ClientWindow(newClient);

            if (window.ShowDialog() == true)
            {
                _context.Clients.Add(newClient);
                _context.SaveChanges();
                LoadData(TxtSearch.Text);
            }
        }

        public void EditClient()
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                var window = new ClientWindow(selectedClient);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadData(TxtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void DeleteClient()
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить клиента {selectedClient.ContactName}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Clients.Remove(selectedClient);
                        _context.SaveChanges();
                        LoadData(TxtSearch.Text);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Не удалось удалить клиента. Возможно, существуют связанные заказы.\nОшибка: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}