using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace WpfApp1
{
    internal class ExcelExporter
    {
        public static void ExportToExcel(List<Edinica> data)
        {
            try
            {
                // Просто сообщение, что функция будет реализована позже
                MessageBox.Show("Экспорт в Excel будет реализован в следующей версии",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                // Или можно сохранить в CSV для простоты
                SaveToCSV(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SaveToCSV(List<Edinica> data)
        {
            try
            {
                string fileName = $"Донор21_Экспорт_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    fileName);

                using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Заголовки
                    writer.WriteLine("ID донора;Компонент;Статус;Дата сбора;Группа крови;Резус-фактор");

                    // Данные
                    foreach (var item in data)
                    {
                        writer.WriteLine($"{item.DonorID};{item.Component};{item.Status};" +
                                        $"{item.Date_Sbora:dd.MM.yyyy};{item.BloodGroup};{item.RhString}");
                    }
                }

                MessageBox.Show($"Данные экспортированы в CSV файл:\n{filePath}",
                    "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения CSV: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
