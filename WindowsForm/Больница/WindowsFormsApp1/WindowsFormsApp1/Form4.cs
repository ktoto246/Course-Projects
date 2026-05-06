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
    public partial class Form4 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=MedicalClinic;Integrated Security=True;Encrypt=False";
        public Form4()
        {
            InitializeComponent();
            LoadMedicalRecordsData();
            LoadComboBoxData();
        }
        private void LoadMedicalRecordsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        м.id_записи as 'ID',
                        п.фамилия + ' ' + п.имя AS 'Пациент',
                        с.фамилия + ' ' + с.имя AS 'Врач',
                        с.должность as 'Должность',
                        м.дата_приема as 'Дата приема',
                        м.жалобы as 'Жалобы',
                        м.диагноз as 'Диагноз',
                        м.назначенное_лечение as 'Лечение',
                        м.статус as 'Статус',
                        м.следующая_явка as 'Следующая явка'
                    FROM Медицинские_записи м
                    JOIN Пациенты п ON м.id_пациента = п.id_пациента
                    JOIN Сотрудники с ON м.id_врача = с.id_сотрудника";
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
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    // Загрузка пациентов
                    string queryPatients = "SELECT id_пациента, фамилия + ' ' + имя AS ФИО FROM Пациенты";
                    SqlDataAdapter adapterPatients = new SqlDataAdapter(queryPatients, conn);
                    DataTable dtPatients = new DataTable();
                    adapterPatients.Fill(dtPatients);

                    cmbPatient.DataSource = dtPatients;
                    cmbPatient.DisplayMember = "ФИО";
                    cmbPatient.ValueMember = "id_пациента";

                    // Загрузка врачей
                    string queryDoctors = "SELECT id_сотрудника, фамилия + ' ' + имя AS ФИО FROM Сотрудники WHERE должность != 'Медсестра' AND должность != 'Администратор'";
                    SqlDataAdapter adapterDoctors = new SqlDataAdapter(queryDoctors, conn);
                    DataTable dtDoctors = new DataTable();
                    adapterDoctors.Fill(dtDoctors);

                    cmbDoctor.DataSource = dtDoctors;
                    cmbDoctor.DisplayMember = "ФИО";
                    cmbDoctor.ValueMember = "id_сотрудника";

                    // Установка начального статуса
                    cmbStatus.SelectedIndex = 2; // "Завершен"
                }
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
                string query = @"INSERT INTO Медицинские_записи 
                    (id_пациента, id_врача, жалобы, диагноз, назначенное_лечение, статус, следующая_явка) 
                    VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (cmbPatient.SelectedValue == null || cmbDoctor.SelectedValue == null ||
                        string.IsNullOrEmpty(txtDiagnosis.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля: Пациент, Врач, Диагноз!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", cmbPatient.SelectedValue);
                        command.Parameters.AddWithValue("@Value2", cmbDoctor.SelectedValue);
                        command.Parameters.AddWithValue("@Value3", txtComplaints.Text);
                        command.Parameters.AddWithValue("@Value4", txtDiagnosis.Text);
                        command.Parameters.AddWithValue("@Value5", txtTreatment.Text);
                        command.Parameters.AddWithValue("@Value6", cmbStatus.Text);

                        // Обработка следующей явки (может быть пустой)
                        if (dtpNextVisit.Value > DateTime.Now)
                            command.Parameters.AddWithValue("@Value7", dtpNextVisit.Value);
                        else
                            command.Parameters.AddWithValue("@Value7", DBNull.Value);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Медицинская запись добавлена!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadMedicalRecordsData();
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
                if (string.IsNullOrEmpty(txtRecordId.Text))
                {
                    MessageBox.Show("Введите ID записи для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtRecordId.Text, out int recordId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "DELETE FROM Медицинские_записи WHERE id_записи = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить запись с ID: " + recordId + "?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", recordId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Запись удалена!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtRecordId.Clear();
                                LoadMedicalRecordsData();
                            }
                            else
                            {
                                MessageBox.Show("Запись с указанным ID не найдена", "Ошибка",
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
                            м.id_записи as 'ID',
                            п.фамилия + ' ' + п.имя AS 'Пациент',
                            с.фамилия + ' ' + с.имя AS 'Врач',
                            с.должность as 'Должность',
                            м.дата_приема as 'Дата приема',
                            м.жалобы as 'Жалобы',
                            м.диагноз as 'Диагноз',
                            м.назначенное_лечение as 'Лечение',
                            м.статус as 'Статус',
                            м.следующая_явка as 'Следующая явка'
                        FROM Медицинские_записи м
                        JOIN Пациенты п ON м.id_пациента = п.id_пациента
                        JOIN Сотрудники с ON м.id_врача = с.id_сотрудника
                        WHERE п.фамилия LIKE @SearchText + '%' OR 
                              с.фамилия LIKE @SearchText + '%' OR
                              м.диагноз LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Записи не найдены!", "Информация",
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
                LoadMedicalRecordsData();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMedicalRecordsData();
            txtSearch.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ClearFields()
        {
            txtComplaints.Clear();
            txtDiagnosis.Clear();
            txtTreatment.Clear();
            cmbStatus.SelectedIndex = 2; // "Завершен"
            dtpNextVisit.Value = DateTime.Now.AddDays(7); // Следующая явка через неделю
        }
    }
}
