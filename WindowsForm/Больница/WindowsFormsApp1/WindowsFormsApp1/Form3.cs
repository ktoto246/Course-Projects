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
    public partial class Form3 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=MedicalClinic;Integrated Security=True;Encrypt=False";
        public Form3()
        {
            InitializeComponent();
            LoadPatientsData();
        }
        private void LoadPatientsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        id_пациента,
                        фамилия,
                        имя,
                        отчество,
                        дата_рождения,
                        пол,
                        адрес,
                        телефон,
                        страховой_полис,
                        дата_регистрации
                    FROM Пациенты";
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
                string query = @"INSERT INTO Пациенты 
                    (фамилия, имя, отчество, дата_рождения, пол, адрес, телефон, страховой_полис) 
                    VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtFirstName.Text) ||
                        string.IsNullOrEmpty(cmbGender.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Пол!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtLastName.Text);
                        command.Parameters.AddWithValue("@Value2", txtFirstName.Text);
                        command.Parameters.AddWithValue("@Value3", txtMiddleName.Text);
                        command.Parameters.AddWithValue("@Value4", dtpBirthDate.Value);
                        command.Parameters.AddWithValue("@Value5", cmbGender.Text);
                        command.Parameters.AddWithValue("@Value6", txtAddress.Text);
                        command.Parameters.AddWithValue("@Value7", txtPhone.Text);
                        command.Parameters.AddWithValue("@Value8", txtInsurance.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            LoadPatientsData();
                            txtLastName.Clear();
                            txtFirstName.Clear();
                            txtMiddleName.Clear();
                            txtAddress.Clear();
                            txtPhone.Clear();
                            txtInsurance.Clear();
                            cmbGender.SelectedIndex = -1;
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
                if (string.IsNullOrEmpty(txtPatientId.Text))
                {
                    MessageBox.Show("Введите ID пациента для удаления!");
                    return;
                }

                string query = "DELETE FROM Пациенты WHERE id_пациента = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить этого пациента?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", txtPatientId.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                txtPatientId.Clear();
                                LoadPatientsData();
                            }
                            else
                            {
                                MessageBox.Show("Пациент с указанным ID не найден");
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
            LoadPatientsData();
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
                            id_пациента,
                            фамилия,
                            имя,
                            отчество,
                            дата_рождения,
                            пол,
                            адрес,
                            телефон,
                            страховой_полис,
                            дата_регистрации
                        FROM Пациенты
                        WHERE фамилия LIKE @SearchText + '%' OR 
                              имя LIKE @SearchText + '%' OR
                              страховой_полис LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Пациенты не найдены!");
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
                LoadPatientsData();
            }
        }
    }
}
