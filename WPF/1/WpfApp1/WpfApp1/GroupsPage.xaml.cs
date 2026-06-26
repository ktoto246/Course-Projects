using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class GroupsPage : Page
    {
        private CollegeContext _context;
        private Group _selectedGroup;

        public GroupsPage()
        {
            InitializeComponent();
            _context = new CollegeContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            GroupsGrid.ItemsSource = _context.Groups.Include(g => g.Curator).ToList();
            CmbCurator.ItemsSource = _context.Teachers.ToList();
        }

        private void GroupsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsGrid.SelectedItem is Group group)
            {
                _selectedGroup = group;

                TxtName.Text = group.Name;
                TxtSpecialty.Text = group.Specialty;
                TxtSpecialtyCode.Text = group.SpecialtyCode;
                TxtCourse.Text = group.Course.ToString();
                CmbCurator.SelectedValue = group.CuratorId;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtSpecialty.Text) ||
                string.IsNullOrWhiteSpace(TxtSpecialtyCode.Text) ||
                string.IsNullOrWhiteSpace(TxtCourse.Text))
            {
                MessageBox.Show("Заполни все обязательные поля со звездочкой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtCourse.Text, out _))
            {
                MessageBox.Show("Курс должен быть целым числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newGroup = new Group
            {
                Name = TxtName.Text.Trim(),
                Specialty = TxtSpecialty.Text.Trim(),
                SpecialtyCode = TxtSpecialtyCode.Text.Trim(),
                Course = int.Parse(TxtCourse.Text.Trim()),
                CuratorId = CmbCurator.SelectedValue as int?
            };

            try
            {
                _context.Groups.Add(newGroup);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGroup == null)
            {
                MessageBox.Show("Сначала выбери группу в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedGroup.Name = TxtName.Text.Trim();
                _selectedGroup.Specialty = TxtSpecialty.Text.Trim();
                _selectedGroup.SpecialtyCode = TxtSpecialtyCode.Text.Trim();
                _selectedGroup.Course = int.Parse(TxtCourse.Text.Trim());
                _selectedGroup.CuratorId = CmbCurator.SelectedValue as int?;

                _context.Groups.Update(_selectedGroup);
                _context.SaveChanges();

                GroupsGrid.ItemsSource = null;
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGroup == null)
            {
                MessageBox.Show("Выбери группу для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Снести выбранную группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Groups.Remove(_selectedGroup);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нельзя удалить группу, пока в ней есть студенты или привязка к расписанию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedGroup = null;
            GroupsGrid.SelectedItem = null;

            TxtName.Clear();
            TxtSpecialty.Clear();
            TxtSpecialtyCode.Clear();
            TxtCourse.Clear();
            CmbCurator.SelectedItem = null;
        }
    }
}