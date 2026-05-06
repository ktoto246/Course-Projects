using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Детская_школа_искусств_Балашов;Integrated Security=True;Encrypt=False";
        public Form2()
        {
            InitializeComponent();
            LoadEmployeesData();
        }
        private void LoadEmployeesData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    id_сотрудника,
                    фамилия,
                    имя,
                    отчество,
                    должность,
                    телефон,
                    email,
                    дата_приема
                FROM сотрудники";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewEmployees.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO сотрудники 
                (фамилия, имя, отчество, должность, телефон, email, дата_приема) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1",textBoxLastName.Text);
                        command.Parameters.AddWithValue("@Value2", textBoxFirstName.Text);
                        command.Parameters.AddWithValue("@Value3", textBoxMiddleName.Text);
                        command.Parameters.AddWithValue("@Value4", textBoxPosition.Text);
                        command.Parameters.AddWithValue("@Value5", textBoxPhone.Text);
                        command.Parameters.AddWithValue("@Value6", textBoxEmail.Text);
                        command.Parameters.AddWithValue("@Value7", dateTimePickerHireDate.Value);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            textBoxLastName.Clear();
                            textBoxFirstName.Clear();
                            textBoxMiddleName.Clear();
                            textBoxPosition.Clear();
                            textBoxPhone.Clear();
                            textBoxEmail.Clear();
                            LoadEmployeesData();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при добавлении данных");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "DELETE FROM сотрудники WHERE id_сотрудника = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult != DialogResult.Yes)
                        return;

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", textBox2.Text);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные удалены!");
                            textBox2.Clear();
                            LoadEmployeesData();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении данных");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Запись не найдена " + ex.Message);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        conn.Open();
                        string query = @"SELECT 
            id_сотрудника,
            фамилия,
            имя,
            отчество,
            должность,
            телефон,
            email,
            дата_приема
        FROM сотрудники
        WHERE фамилия LIKE @SearchText + '%' OR имя LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Сотрудник не найден!");
                        }

                        dataGridViewEmployees.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message);
                    }
                }
            }
        }
    }
}
