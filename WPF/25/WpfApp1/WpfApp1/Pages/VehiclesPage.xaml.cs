using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для VehiclesPage.xaml
    /// </summary>
    public partial class VehiclesPage : Page
    {
        private Vehicle? _selectedVehicle;
        public VehiclesPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (var db = new BmtcContext())
            {
                VehiclesGrid.ItemsSource = db.Vehicles.Include(v => v.Client).ToList();
                CmbClient.ItemsSource = db.Clients.ToList();
            }
        }

        private void VehiclesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem is Vehicle vehicle)
            {
                _selectedVehicle = vehicle;
                TxtBrand.Text = vehicle.Brand;
                TxtModel.Text = vehicle.Model;
                TxtVIN.Text = vehicle.VIN;
                TxtRegNumber.Text = vehicle.RegNumber;

                CmbClient.SelectedValue = vehicle.ClientId;

                foreach (ComboBoxItem item in CmbCategory.Items)
                {
                    if (item.Content.ToString()!.StartsWith(vehicle.Category))
                    {
                        CmbCategory.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private bool ValidateInput()
        {
            if (CmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите владельца из списка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtVIN.Text) || string.IsNullOrWhiteSpace(TxtRegNumber.Text) ||
                string.IsNullOrWhiteSpace(TxtBrand.Text) || string.IsNullOrWhiteSpace(TxtModel.Text))
            {
                MessageBox.Show("Заполните все текстовые поля (Марка, Модель, VIN, Гос. номер)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                string categoryFull = ((ComboBoxItem)CmbCategory.SelectedItem).Content.ToString()!;
                string categoryCode = categoryFull.Substring(0, 2).Trim();

                var newVehicle = new Vehicle
                {
                    ClientId = (int)CmbClient.SelectedValue,
                    Brand = TxtBrand.Text.Trim(),
                    Model = TxtModel.Text.Trim(),
                    VIN = TxtVIN.Text.Trim(),
                    RegNumber = TxtRegNumber.Text.Trim(),
                    Category = categoryCode
                };

                db.Vehicles.Add(newVehicle);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Выберите ТС для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                var vehicleToUpdate = db.Vehicles.FirstOrDefault(x => x.Id == _selectedVehicle.Id);
                if (vehicleToUpdate != null)
                {
                    string categoryFull = ((ComboBoxItem)CmbCategory.SelectedItem).Content.ToString()!;

                    vehicleToUpdate.ClientId = (int)CmbClient.SelectedValue;
                    vehicleToUpdate.Brand = TxtBrand.Text.Trim();
                    vehicleToUpdate.Model = TxtModel.Text.Trim();
                    vehicleToUpdate.VIN = TxtVIN.Text.Trim();
                    vehicleToUpdate.RegNumber = TxtRegNumber.Text.Trim();
                    vehicleToUpdate.Category = categoryFull.Substring(0, 2).Trim();

                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedVehicle == null)
            {
                MessageBox.Show("Выберите ТС для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить автомобиль {_selectedVehicle.Brand} {_selectedVehicle.RegNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new BmtcContext())
                {
                    var vehicleToDelete = db.Vehicles.FirstOrDefault(x => x.Id == _selectedVehicle.Id);
                    if (vehicleToDelete != null)
                    {
                        db.Vehicles.Remove(vehicleToDelete);
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
            _selectedVehicle = null;
            VehiclesGrid.SelectedItem = null;
            TxtBrand.Clear();
            TxtModel.Clear();
            TxtVIN.Clear();
            TxtRegNumber.Clear();
            CmbClient.SelectedItem = null;
            CmbCategory.SelectedIndex = 0;
        }
    }
}