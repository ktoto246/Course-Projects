using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=КФХ_Горбов_НИ;Integrated Security=True;Encrypt=False";
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
                        дата_приема,
                        оклад,
                        телефон
                    FROM сотрудники";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
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
                    (фамилия, имя, отчество, должность, дата_приема, оклад, телефон) 
                    VALUES (@фамилия, @имя, @отчество, @должность, @дата_приема, @оклад, @телефон)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(textBoxФамилия.Text) ||
                        string.IsNullOrEmpty(textBoxИмя.Text) ||
                        string.IsNullOrEmpty(textBoxДолжность.Text) ||
                        string.IsNullOrEmpty(textBoxОклад.Text))
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@фамилия", textBoxФамилия.Text);
                        command.Parameters.AddWithValue("@имя", textBoxИмя.Text);
                        command.Parameters.AddWithValue("@отчество", textBoxОтчество.Text ?? "");
                        command.Parameters.AddWithValue("@должность", textBoxДолжность.Text);
                        command.Parameters.AddWithValue("@дата_приема", dateTimePickerПрием.Value);
                        command.Parameters.AddWithValue("@оклад", decimal.Parse(textBoxОклад.Text));
                        command.Parameters.AddWithValue("@телефон", textBoxТелефон.Text ?? "");

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            // Очистка полей
                            textBoxФамилия.Clear();
                            textBoxИмя.Clear();
                            textBoxОтчество.Clear();
                            textBoxДолжность.Clear();
                            textBoxОклад.Clear();
                            textBoxТелефон.Clear();
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
                string query = "DELETE FROM сотрудники WHERE id_сотрудника = @id_сотрудника";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@id_сотрудника", textBoxID.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                textBoxID.Clear();
                                LoadEmployeesData();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при удалении данных");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Запись не найдена " + ex.Message);
            }
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
                            дата_приема,
                            оклад,
                            телефон
                        FROM сотрудники
                        WHERE фамилия LIKE @SearchText + '%' OR имя LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
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
            else
            {
                LoadEmployeesData();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
