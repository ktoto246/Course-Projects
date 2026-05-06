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
        private string connection = "Data Source=PC;Initial Catalog=MedicalClinic;Integrated Security=True;Encrypt=False";
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
                        специализация,
                        телефон,
                        email,
                        дата_приема,
                        статус
                    FROM Сотрудники";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO Сотрудники 
                    (фамилия, имя, отчество, должность, специализация, телефон, email, дата_приема, статус) 
                    VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtFirstName.Text) ||
                        string.IsNullOrEmpty(cmbPosition.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Должность!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtLastName.Text);
                        command.Parameters.AddWithValue("@Value2", txtFirstName.Text);
                        command.Parameters.AddWithValue("@Value3", txtMiddleName.Text);
                        command.Parameters.AddWithValue("@Value4", cmbPosition.Text);
                        command.Parameters.AddWithValue("@Value5", txtSpecialization.Text);
                        command.Parameters.AddWithValue("@Value6", txtPhone.Text);
                        command.Parameters.AddWithValue("@Value7", txtEmail.Text);
                        command.Parameters.AddWithValue("@Value8", dtpHireDate.Value);
                        command.Parameters.AddWithValue("@Value9", cmbStatus.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            LoadEmployeesData();
                            txtLastName.Clear();
                            txtFirstName.Clear();
                            txtMiddleName.Clear();
                            txtSpecialization.Clear();
                            txtPhone.Clear();
                            txtEmail.Clear();
                            cmbPosition.SelectedIndex = -1;
                            cmbStatus.SelectedIndex = -1;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtEmployeeId.Text))
                {
                    MessageBox.Show("Введите ID сотрудника для удаления!");
                    return;
                }

                string query = "DELETE FROM Сотрудники WHERE id_сотрудника = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", txtEmployeeId.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                txtEmployeeId.Clear();
                                LoadEmployeesData();
                            }
                            else
                            {
                                MessageBox.Show("Сотрудник с указанным ID не найден");
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
            txtSearch.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                            id_сотрудника,
                            фамилия,
                            имя,
                            отчество,
                            должность,
                            специализация,
                            телефон,
                            email,
                            дата_приема,
                            статус
                        FROM Сотрудники
                        WHERE фамилия LIKE @SearchText + '%' OR 
                              имя LIKE @SearchText + '%' OR
                              специализация LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Сотрудники не найдены!");
                        }

                        dataGridView1.DataSource = table;
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
    }
}
