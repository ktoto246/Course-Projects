using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class JournalPage : Page
    {
        private CollegeContext _context;
        private Attendance _selectedAttendance;

        public JournalPage()
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
            JournalGrid.ItemsSource = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Subject)
                .ToList();

            CmbStudent.ItemsSource = _context.Students
                .Select(s => new { s.StudentId, FullName = s.LastName + " " + s.FirstName })
                .ToList();

            CmbSchedule.ItemsSource = _context.Schedules
                .Include(s => s.Subject)
                .Select(s => new { s.ScheduleId, Info = s.ClassDate.ToString("dd.MM.yyyy") + " - " + s.Subject.Name })
                .ToList();
        }

        private void JournalGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JournalGrid.SelectedItem is Attendance attendance)
            {
                _selectedAttendance = attendance;

                CmbStudent.SelectedValue = attendance.StudentId;
                CmbSchedule.SelectedValue = attendance.ScheduleId;
                TxtGrade.Text = attendance.Grade?.ToString() ?? "";
                DpEntryDate.SelectedDate = attendance.EntryDate;

                foreach (ComboBoxItem item in CmbStatus.Items)
                {
                    if (item.Content.ToString() == attendance.Status)
                    {
                        CmbStatus.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem item in CmbControlType.Items)
                {
                    if (item.Content.ToString() == attendance.ControlType)
                    {
                        CmbControlType.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private bool ValidateForm()
        {
            if (CmbStudent.SelectedValue == null ||
                CmbSchedule.SelectedValue == null ||
                CmbStatus.SelectedItem == null ||
                CmbControlType.SelectedItem == null ||
                DpEntryDate.SelectedDate == null)
            {
                MessageBox.Show("Заполни все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TxtGrade.Text) && !int.TryParse(TxtGrade.Text, out _))
            {
                MessageBox.Show("Оценка должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newAttendance = new Attendance
            {
                StudentId = (int)CmbStudent.SelectedValue,
                ScheduleId = (int)CmbSchedule.SelectedValue,
                Status = ((ComboBoxItem)CmbStatus.SelectedItem).Content.ToString(),
                Grade = string.IsNullOrWhiteSpace(TxtGrade.Text) ? (int?)null : int.Parse(TxtGrade.Text.Trim()),
                ControlType = ((ComboBoxItem)CmbControlType.SelectedItem).Content.ToString(),
                EntryDate = DpEntryDate.SelectedDate.Value
            };

            try
            {
                _context.Attendances.Add(newAttendance);
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
            if (_selectedAttendance == null)
            {
                MessageBox.Show("Выбери запись в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedAttendance.StudentId = (int)CmbStudent.SelectedValue;
                _selectedAttendance.ScheduleId = (int)CmbSchedule.SelectedValue;
                _selectedAttendance.Status = ((ComboBoxItem)CmbStatus.SelectedItem).Content.ToString();
                _selectedAttendance.Grade = string.IsNullOrWhiteSpace(TxtGrade.Text) ? (int?)null : int.Parse(TxtGrade.Text.Trim());
                _selectedAttendance.ControlType = ((ComboBoxItem)CmbControlType.SelectedItem).Content.ToString();
                _selectedAttendance.EntryDate = DpEntryDate.SelectedDate.Value;

                _context.Attendances.Update(_selectedAttendance);
                _context.SaveChanges();

                JournalGrid.ItemsSource = null;
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
            if (_selectedAttendance == null)
            {
                MessageBox.Show("Выбери запись для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Удалить запись об успеваемости?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Attendances.Remove(_selectedAttendance);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedAttendance = null;
            JournalGrid.SelectedItem = null;

            CmbStudent.SelectedItem = null;
            CmbSchedule.SelectedItem = null;
            CmbStatus.SelectedItem = null;
            TxtGrade.Clear();
            CmbControlType.SelectedItem = null;
            DpEntryDate.SelectedDate = null;
        }
    }
}