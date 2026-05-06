using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{

    internal class DatabaseHelper
    {
        private static string _connectionString = @"Data Source=DESKTOP-U8DS2P9\SQLEXPRESS;Initial Catalog=bd;Integrated Security=True";

        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        //Список обектов
        public static List<Edinica> GetEdinici(string component = null)
        {
            List<Edinica> edinici = new List<Edinica>();

            string query = @"
                SELECT e.*, s.Status 
                FROM Edinica e 
                INNER JOIN Status s ON e.FK_Status = s.ID_Status
                WHERE (@Component IS NULL OR e.Component = @Component)";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(component))
                        cmd.Parameters.AddWithValue("@Component", component);
                    else
                        cmd.Parameters.AddWithValue("@Component", DBNull.Value);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Edinica ed = new Edinica
                        {
                            ID_Edinica = Convert.ToInt32(reader["ID_Edinica"]),
                            DonorID = Convert.ToInt32(reader["DonorID"]),
                            Component = reader["Component"].ToString(),
                            FK_Status = Convert.ToInt32(reader["FK_Status"]),
                            Date_Sbora = Convert.ToDateTime(reader["Date_Sbora"]),
                            BloodGroup = reader["BloodGroup"].ToString(),
                            StatusObj = new Status
                            {
                                ID_Status = Convert.ToInt32(reader["FK_Status"]),
                                StatusName = reader["Status"].ToString()
                            }
                        };

                        if (reader["Date_Freeze"] != DBNull.Value)
                            ed.Date_Freeze = Convert.ToDateTime(reader["Date_Freeze"]);

                        if (reader["Rh"] != DBNull.Value)
                            ed.Rh = Convert.ToBoolean(reader["Rh"]);

                        edinici.Add(ed);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return edinici;
        }
        //Автоматическое обновление статусов
        public static bool UpdateStatus(int idEdinica, int newStatusId)
        {
            string query = "UPDATE Edinica SET FK_Status = @Status WHERE ID_Edinica = @ID";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", newStatusId);
                    cmd.Parameters.AddWithValue("@ID", idEdinica);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static void ExecuteUpdateStatusesAutomatically()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = @"
                UPDATE Edinica
                SET FK_Status = 2
                WHERE FK_Status = 1 
                    AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 90;
                UPDATE Edinica
                SET FK_Status = 5
                WHERE FK_Status = 2
                    AND Component = 'Эритроциты'
                    AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 180;
                UPDATE Edinica
                SET FK_Status = 5
                WHERE FK_Status = 2
                    AND Component = 'Плазма'
                    AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 240;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка автоматического обновления статусов: {ex.Message}");
            }
        }
        public static bool AddEdinica(Edinica edinica)
        {
            string query = @"
                INSERT INTO Edinica 
                (DonorID, Component, FK_Status, Date_Sbora, Date_Freeze, BloodGroup, Rh)
                VALUES (@DonorID, @Component, @Status, @Date_Sbora, @Date_Freeze, @BloodGroup, @Rh)";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DonorID", edinica.DonorID);
                    cmd.Parameters.AddWithValue("@Component", edinica.Component);
                    cmd.Parameters.AddWithValue("@Status", edinica.FK_Status);
                    cmd.Parameters.AddWithValue("@Date_Sbora", edinica.Date_Sbora);

                    if (edinica.Date_Freeze.HasValue)
                        cmd.Parameters.AddWithValue("@Date_Freeze", edinica.Date_Freeze.Value);
                    else
                        cmd.Parameters.AddWithValue("@Date_Freeze", DBNull.Value);
                    cmd.Parameters.AddWithValue("@BloodGroup", edinica.BloodGroup);
                    if (edinica.Rh.HasValue)
                        cmd.Parameters.AddWithValue("@Rh", edinica.Rh.Value);
                    else
                        cmd.Parameters.AddWithValue("@Rh", DBNull.Value);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления записи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static bool DeleteEdinica(int idEdinica)
        {
            string query = "DELETE FROM Edinica WHERE ID_Edinica = @ID";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", idEdinica);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления записи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public static void CheckAndUpdateExpiredStatuses()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string query = @"
                -- Проверка истечения сроков хранения
                UPDATE Edinica
                SET FK_Status = 5 -- Бракованная
                WHERE FK_Status IN (2, 3) -- Законсервирована или Пригодна для использования
                    AND (
                        (Component = 'Эритроциты' AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 180) OR
                        (Component = 'Плазма' AND DATEDIFF(day, Date_Sbora, GETDATE()) >= 240)
                    )";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка проверки сроков: {ex.Message}");
            }
        }
    }
}
