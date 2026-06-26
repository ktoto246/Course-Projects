using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class TeachersPage : Page
    {
        private CollegeContext _context;
        private Teacher _selectedTeacher;

        public TeachersPage()
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
            TeachersGrid.ItemsSource = _context.Teachers.ToList();
        }

        private void TeachersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeachersGrid.SelectedItem is Teacher teacher)
            {
                _selectedTeacher = teacher;

                TxtLastName.Text = teacher.LastName;
                TxtFirstName.Text = teacher.FirstName;
                TxtMiddleName.Text = teacher.MiddleName;
                TxtPhone.Text = teacher.Phone;
                TxtEmail.Text = teacher.Email;
                TxtPosition.Text = teacher.Position;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtPosition.Text))
            {
                MessageBox.Show("Заполни обязательные поля: Фамилия, Имя и Должность!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newTeacher = new Teacher
            {
                LastName = TxtLastName.Text.Trim(),
                FirstName = TxtFirstName.Text.Trim(),
                MiddleName = TxtMiddleName.Text.Trim(),
                Phone = TxtPhone.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                Position = TxtPosition.Text.Trim()
            };

            try
            {
                _context.Teachers.Add(newTeacher);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTeacher == null)
            {
                MessageBox.Show("Сначала выбери преподавателя в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedTeacher.LastName = TxtLastName.Text.Trim();
                _selectedTeacher.FirstName = TxtFirstName.Text.Trim();
                _selectedTeacher.MiddleName = TxtMiddleName.Text.Trim();
                _selectedTeacher.Phone = TxtPhone.Text.Trim();
                _selectedTeacher.Email = TxtEmail.Text.Trim();
                _selectedTeacher.Position = TxtPosition.Text.Trim();

                _context.Teachers.Update(_selectedTeacher);
                _context.SaveChanges();

                TeachersGrid.ItemsSource = null;
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTeacher == null)
            {
                MessageBox.Show("Выбери преподавателя для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Точно хочешь снести препода {_selectedTeacher.LastName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Teachers.Remove(_selectedTeacher);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нельзя удалить препода, пока он привязан к группе или расписанию!", "Ошибка внешнего ключа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedTeacher = null;
            TeachersGrid.SelectedItem = null;

            TxtLastName.Clear();
            TxtFirstName.Clear();
            TxtMiddleName.Clear();
            TxtPhone.Clear();
            TxtEmail.Clear();
            TxtPosition.Clear();
        }
    }
}