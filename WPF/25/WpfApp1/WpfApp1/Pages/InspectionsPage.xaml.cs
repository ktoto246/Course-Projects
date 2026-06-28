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
    /// Логика взаимодействия для InspectionsPage.xaml
    /// </summary>
    public partial class InspectionsPage : Page
    {
        private Inspection _selectedInspection;
        public InspectionsPage()
        {
            InitializeComponent();
            LoadData();
            DpDate.SelectedDate = DateTime.Now;
        }
        private void LoadData()
        {
            using (var db = new BmtcContext())
            {
                InspectionsGrid.ItemsSource = db.Inspections
                    .Include(i => i.Vehicle)
                    .Include(i => i.Service)
                    .Include(i => i.Employee)
                    .ToList();

                CmbVehicle.ItemsSource = db.Vehicles.ToList();
                CmbService.ItemsSource = db.Services.ToList();
                CmbEmployee.ItemsSource = db.Employees.ToList();
            }
        }

        private void InspectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InspectionsGrid.SelectedItem is Inspection inspection)
            {
                _selectedInspection = inspection;

                CmbVehicle.SelectedValue = inspection.VehicleId;
                CmbService.SelectedValue = inspection.ServiceId;
                CmbEmployee.SelectedValue = inspection.EmployeeId;

                DpDate.SelectedDate = inspection.InspectionDate.Date;
                TxtTime.Text = inspection.InspectionDate.ToString("HH:mm");

                ChkIsPassed.IsChecked = inspection.IsPassed;
                TxtComments.Text = inspection.Comments;
            }
        }

        private bool ValidateInput()
        {
            if (CmbVehicle.SelectedValue == null || CmbService.SelectedValue == null || CmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выберите автомобиль, услугу и диагноста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!TimeSpan.TryParse(TxtTime.Text, out _))
            {
                MessageBox.Show("Введите корректное время в формате ЧЧ:ММ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private DateTime GetCombineDateTime()
        {
            var date = DpDate.SelectedDate!.Value;
            var time = TimeSpan.Parse(TxtTime.Text);
            return date.Add(time);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                var newInspection = new Inspection
                {
                    VehicleId = (int)CmbVehicle.SelectedValue,
                    ServiceId = (int)CmbService.SelectedValue,
                    EmployeeId = (int)CmbEmployee.SelectedValue,
                    InspectionDate = GetCombineDateTime(),
                    IsPassed = ChkIsPassed.IsChecked ?? false,
                    Comments = TxtComments.Text.Trim()
                };

                db.Inspections.Add(newInspection);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedInspection == null)
            {
                MessageBox.Show("Выберите запись для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                var inspectionToUpdate = db.Inspections.FirstOrDefault(x => x.Id == _selectedInspection.Id);
                if (inspectionToUpdate != null)
                {
                    inspectionToUpdate.VehicleId = (int)CmbVehicle.SelectedValue;
                    inspectionToUpdate.ServiceId = (int)CmbService.SelectedValue;
                    inspectionToUpdate.EmployeeId = (int)CmbEmployee.SelectedValue;
                    inspectionToUpdate.InspectionDate = GetCombineDateTime();
                    inspectionToUpdate.IsPassed = ChkIsPassed.IsChecked ?? false;
                    inspectionToUpdate.Comments = TxtComments.Text.Trim();

                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedInspection == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Точно удалить этот техосмотр?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new BmtcContext())
                {
                    var inspectionToDelete = db.Inspections.FirstOrDefault(x => x.Id == _selectedInspection.Id);
                    if (inspectionToDelete != null)
                    {
                        db.Inspections.Remove(inspectionToDelete);
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
            _selectedInspection = null;
            InspectionsGrid.SelectedItem = null;

            CmbVehicle.SelectedItem = null;
            CmbService.SelectedItem = null;
            CmbEmployee.SelectedItem = null;

            DpDate.SelectedDate = DateTime.Now;
            TxtTime.Text = "12:00";

            ChkIsPassed.IsChecked = false;
            TxtComments.Clear();
        }
    }
}