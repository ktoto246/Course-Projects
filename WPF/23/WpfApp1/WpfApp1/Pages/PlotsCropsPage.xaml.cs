using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class PlotsCropsPage : Page
    {
        private AppDbContext _context;

        public PlotsCropsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            CropsGrid.ItemsSource = _context.Crops.ToList();

            PlotsGrid.ItemsSource = _context.Plots
                .Include(p => p.ResponsibleEmployee)
                .Include(p => p.Crop)
                .ToList();

            cmbEmployee.ItemsSource = _context.Employees.ToList();

            cmbCrop.ItemsSource = _context.Crops.ToList();
        }

        private void CropsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CropsGrid.SelectedItem is Crop selected)
            {
                txtCropName.Text = selected.Name;
                cmbCropType.Text = selected.Type;
                txtMaturationDays.Text = selected.MaturationDays.ToString();
                txtSeedCost.Text = selected.SeedCostPerHa?.ToString();
                txtYield.Text = selected.ExpectedYieldTHa?.ToString();
            }
        }

        private void BtnAddCrop_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCropInput()) return;

            var newCrop = new Crop
            {
                Name = txtCropName.Text,
                Type = cmbCropType.Text,
                MaturationDays = int.Parse(txtMaturationDays.Text),
                SeedCostPerHa = string.IsNullOrWhiteSpace(txtSeedCost.Text) ? null : decimal.Parse(txtSeedCost.Text),
                ExpectedYieldTHa = string.IsNullOrWhiteSpace(txtYield.Text) ? null : decimal.Parse(txtYield.Text)
            };

            try
            {
                _context.Crops.Add(newCrop);
                _context.SaveChanges();
                ClearCropForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateCrop_Click(object sender, RoutedEventArgs e)
        {
            if (CropsGrid.SelectedItem is Crop selected)
            {
                if (!ValidateCropInput()) return;

                selected.Name = txtCropName.Text;
                selected.Type = cmbCropType.Text;
                selected.MaturationDays = int.Parse(txtMaturationDays.Text);
                selected.SeedCostPerHa = string.IsNullOrWhiteSpace(txtSeedCost.Text) ? null : decimal.Parse(txtSeedCost.Text);
                selected.ExpectedYieldTHa = string.IsNullOrWhiteSpace(txtYield.Text) ? null : decimal.Parse(txtYield.Text);

                try
                {
                    _context.SaveChanges();
                    LoadData();
                    CropsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteCrop_Click(object sender, RoutedEventArgs e)
        {
            if (CropsGrid.SelectedItem is Crop selected)
            {
                if (MessageBox.Show("Точно удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Crops.Remove(selected);
                        _context.SaveChanges();
                        ClearCropForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnClearCrop_Click(object sender, RoutedEventArgs e)
        {
            ClearCropForm();
        }

        private void ClearCropForm()
        {
            CropsGrid.SelectedItem = null;
            txtCropName.Clear();
            cmbCropType.SelectedIndex = -1;
            txtMaturationDays.Clear();
            txtSeedCost.Clear();
            txtYield.Clear();
        }

        private bool ValidateCropInput()
        {
            if (string.IsNullOrWhiteSpace(txtCropName.Text) || string.IsNullOrWhiteSpace(cmbCropType.Text) || string.IsNullOrWhiteSpace(txtMaturationDays.Text))
            {
                MessageBox.Show("Заполните обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void PlotsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlotsGrid.SelectedItem is Plot selected)
            {
                txtPlotName.Text = selected.Name;
                cmbEmployee.SelectedValue = selected.ResponsibleEmployeeID;
                txtArea.Text = selected.AreaHa.ToString();
                cmbCrop.SelectedValue = selected.CropID;
                dpPlantingDate.SelectedDate = selected.PlantingDate;
                dpHarvestDate.SelectedDate = selected.PlannedHarvestDate;
            }
        }

        private void BtnAddPlot_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePlotInput()) return;

            var newPlot = new Plot
            {
                Name = txtPlotName.Text,
                ResponsibleEmployeeID = (int)cmbEmployee.SelectedValue,
                AreaHa = decimal.Parse(txtArea.Text),
                CropID = (int?)cmbCrop.SelectedValue,
                PlantingDate = dpPlantingDate.SelectedDate,
                PlannedHarvestDate = dpHarvestDate.SelectedDate
            };

            try
            {
                _context.Plots.Add(newPlot);
                _context.SaveChanges();
                ClearPlotForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdatePlot_Click(object sender, RoutedEventArgs e)
        {
            if (PlotsGrid.SelectedItem is Plot selected)
            {
                if (!ValidatePlotInput()) return;

                selected.Name = txtPlotName.Text;
                selected.ResponsibleEmployeeID = (int)cmbEmployee.SelectedValue;
                selected.AreaHa = decimal.Parse(txtArea.Text);
                selected.CropID = (int?)cmbCrop.SelectedValue;
                selected.PlantingDate = dpPlantingDate.SelectedDate;
                selected.PlannedHarvestDate = dpHarvestDate.SelectedDate;

                try
                {
                    _context.SaveChanges();
                    LoadData();
                    PlotsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeletePlot_Click(object sender, RoutedEventArgs e)
        {
            if (PlotsGrid.SelectedItem is Plot selected)
            {
                if (MessageBox.Show("Точно удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Plots.Remove(selected);
                        _context.SaveChanges();
                        ClearPlotForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnClearPlot_Click(object sender, RoutedEventArgs e)
        {
            ClearPlotForm();
        }

        private void ClearPlotForm()
        {
            PlotsGrid.SelectedItem = null;
            txtPlotName.Clear();
            cmbEmployee.SelectedIndex = -1;
            txtArea.Clear();
            cmbCrop.SelectedIndex = -1;
            dpPlantingDate.SelectedDate = null;
            dpHarvestDate.SelectedDate = null;
        }

        private bool ValidatePlotInput()
        {
            if (string.IsNullOrWhiteSpace(txtPlotName.Text) || cmbEmployee.SelectedValue == null || string.IsNullOrWhiteSpace(txtArea.Text))
            {
                MessageBox.Show("Заполните обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}