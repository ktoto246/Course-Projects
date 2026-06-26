using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class ClientsPage : Page
    {
        private Client _editingClient;

        public ClientsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            using (var context = new ElkiTorgContext())
            {
                var query = context.Clients.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(c => c.CompanyName.Contains(searchText) ||
                                             c.INN.Contains(searchText) ||
                                             c.City.Contains(searchText) ||
                                             c.Phone.Contains(searchText));
                }

                ClientsGrid.ItemsSource = query.ToList();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _editingClient = null;
            PanelTitle.Text = "Новый клиент";
            TxtCompanyName.Text = "";
            TxtINN.Text = "";
            TxtPhone.Text = "";
            TxtCity.Text = "Балашов";

            EditPanel.Visibility = Visibility.Visible;
            ClientsGrid.IsEnabled = false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                _editingClient = selectedClient;
                PanelTitle.Text = "Редактирование";
                TxtCompanyName.Text = selectedClient.CompanyName;
                TxtINN.Text = selectedClient.INN;
                TxtPhone.Text = selectedClient.Phone;
                TxtCity.Text = selectedClient.City;

                EditPanel.Visibility = Visibility.Visible;
                ClientsGrid.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Сначала выберите клиента в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var context = new ElkiTorgContext())
                    {
                        var clientToDelete = context.Clients.Find(selectedClient.ClientID);
                        if (clientToDelete != null)
                        {
                            context.Clients.Remove(clientToDelete);
                            context.SaveChanges();
                            LoadData(TxtSearch.Text);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите клиента.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCompanyName.Text) || string.IsNullOrWhiteSpace(TxtINN.Text))
            {
                MessageBox.Show("Поля Компания и ИНН обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new ElkiTorgContext())
            {
                if (_editingClient == null)
                {
                    var newClient = new Client
                    {
                        CompanyName = TxtCompanyName.Text,
                        INN = TxtINN.Text,
                        Phone = TxtPhone.Text ?? "",
                        City = TxtCity.Text ?? ""
                    };
                    context.Clients.Add(newClient);
                }
                else
                {
                    var clientToUpdate = context.Clients.Find(_editingClient.ClientID);
                    if (clientToUpdate != null)
                    {
                        clientToUpdate.CompanyName = TxtCompanyName.Text;
                        clientToUpdate.INN = TxtINN.Text;
                        clientToUpdate.Phone = TxtPhone.Text ?? "";
                        clientToUpdate.City = TxtCity.Text ?? "";
                    }
                }
                context.SaveChanges();
            }

            ClosePanel();
            LoadData(TxtSearch.Text);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        private void ClosePanel()
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ClientsGrid.IsEnabled = true;
        }
    }
}