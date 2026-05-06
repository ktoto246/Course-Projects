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

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=БАЛКОМХЛЕБПРОДУКТ;Integrated Security=True;Encrypt=False";
        public Form2()
        {
            InitializeComponent();
            LoadEmployeesData();
            LoadComboBoxData();
        }
        private void LoadEmployeesData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    id_сотрудника as 'ID',
                    фамилия as 'Фамилия',
                    имя as 'Имя',
                    отчество as 'Отчество',
                    должность as 'Должность',
                    телефон as 'Телефон',
                    email as 'Email',
                    дата_приема as 'Дата приема'
                FROM сотрудники";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                   dataGridView1.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadComboBoxData()
        {
            try
            {
                cmbPosition.Items.AddRange(new string[] {
                "менеджер по продажам", "логист", "кладовщик", "водитель", "бухгалтер", "директор"
            });
                cmbPosition.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных для выпадающих списков: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO сотрудники 
                (фамилия, имя, отчество, должность, телефон, email) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtFirstName.Text) ||
                        string.IsNullOrEmpty(txtPhone.Text) || cmbPosition.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Телефон, Должность!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtLastName.Text);
                        command.Parameters.AddWithValue("@Value2", txtFirstName.Text);
                        command.Parameters.AddWithValue("@Value3", txtMiddleName.Text);
                        command.Parameters.AddWithValue("@Value4", cmbPosition.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Value5", txtPhone.Text);
                        command.Parameters.AddWithValue("@Value6", txtEmail.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Сотрудник добавлен!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadEmployeesData();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при добавлении данных", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите сотрудника для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int employeeId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string employeeName = selectedRow.Cells["Фамилия"].Value + " " + selectedRow.Cells["Имя"].Value;

                string query = "DELETE FROM сотрудники WHERE id_сотрудника = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника: {employeeName}?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", employeeId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Сотрудник удален!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadEmployeesData();
                            }
                            else
                            {
                                MessageBox.Show("Сотрудник с указанным ID не найден", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                        id_сотрудника as 'ID',
                        фамилия as 'Фамилия',
                        имя as 'Имя',
                        отчество as 'Отчество',
                        должность as 'Должность',
                        телефон as 'Телефон',
                        email as 'Email',
                        дата_приема as 'Дата приема'
                    FROM сотрудники
                    WHERE фамилия LIKE @SearchText + '%' OR 
                          имя LIKE @SearchText + '%' OR
                          должность LIKE @SearchText + '%' OR
                          телефон LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Сотрудники не найдены!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridView1.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                LoadEmployeesData();
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            cmbPosition.SelectedIndex = 0;
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            LoadEmployeesData();
            txtSearch.Clear();
        }
    }
}
