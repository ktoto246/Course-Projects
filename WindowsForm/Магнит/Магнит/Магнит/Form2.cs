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

namespace Магнит
{
    public partial class Form2 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Магнит_Должности;Integrated Security=True;Encrypt=False";
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
                        с.id_сотрудника,
                        с.фио,
                        с.телефон,
                        с.email,
                        д.название_должности,
                        о.название_отдела,
                        с.дата_приема,
                        с.дата_рождения,
                        с.статус
                    FROM Сотрудники с
                    LEFT JOIN Должности д ON с.id_должности = д.id_должности
                    LEFT JOIN Отделы о ON д.id_отдела = о.id_отдела";
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
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewEmployees.CurrentRow == null)
                {
                    MessageBox.Show("Выберите сотрудника для удаления!");
                    return;
                }

                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    string query = "DELETE FROM Сотрудники WHERE id_сотрудника = @id_сотрудника";

                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            int employeeId = Convert.ToInt32(dataGridViewEmployees.CurrentRow.Cells["id_сотрудника"].Value);
                            command.Parameters.AddWithValue("@id_сотрудника", employeeId);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
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
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        conn.Open();
                        string query = @"SELECT 
                            с.id_сотрудника,
                            с.фио,
                            с.телефон,
                            с.email,
                            д.название_должности,
                            о.название_отдела,
                            с.дата_приема,
                            с.дата_рождения,
                            с.статус
                        FROM Сотрудники с
                        LEFT JOIN Должности д ON с.id_должности = д.id_должности
                        LEFT JOIN Отделы о ON д.id_отдела = о.id_отдела
                        WHERE с.фио LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Сотрудники не найдены!");
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO Сотрудники 
                    (id_должности, фио, телефон, email, дата_приема, дата_рождения, статус) 
                    VALUES (@id_должности, @фио, @телефон, @email, @дата_приема, @дата_рождения, @статус)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtPhone.Text) ||
                        string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(textBox1.Text))
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@id_должности", textBox1.Text);
                        command.Parameters.AddWithValue("@фио", txtFullName.Text);
                        command.Parameters.AddWithValue("@телефон", txtPhone.Text);
                        command.Parameters.AddWithValue("@email", txtEmail.Text);
                        command.Parameters.AddWithValue("@дата_приема", dtpHireDate.Value);
                        command.Parameters.AddWithValue("@дата_рождения", dtpBirthDate.Value);
                        command.Parameters.AddWithValue("@статус", cmbPosition.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
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

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}
