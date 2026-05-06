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
    public partial class Form5 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=БАЛКОМХЛЕБПРОДУКТ;Integrated Security=True;Encrypt=False";
        public Form5()
        {
            InitializeComponent();
            LoadEnterprisesData();
            LoadComboBoxData();
        }
        private void LoadEnterprisesData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    id_предприятия as 'ID',
                    название as 'Название',
                    адрес as 'Адрес',
                    телефон as 'Телефон',
                    тип_предприятия as 'Тип',
                    email as 'Email'
                FROM предприятия_розничной_торговли";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewSupplies.DataSource = table;
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
                cmbType.Items.AddRange(new string[] {
                "магазин", "супермаркет", "гипермаркет", "мини-маркет", "торговая точка"
            });
                cmbType.SelectedIndex = 0;
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
                string query = @"INSERT INTO предприятия_розничной_торговли 
                (название, адрес, телефон, тип_предприятия, email) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtAddress.Text) ||
                        string.IsNullOrEmpty(txtPhone.Text) || cmbType.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните обязательные поля: Название, Адрес, Телефон, Тип!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtName.Text);
                        command.Parameters.AddWithValue("@Value2", txtAddress.Text);
                        command.Parameters.AddWithValue("@Value3", txtPhone.Text);
                        command.Parameters.AddWithValue("@Value4", cmbType.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Value5", txtEmail.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Предприятие добавлено!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadEnterprisesData();
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
                if (dataGridViewSupplies.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите предприятие для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dataGridViewSupplies.SelectedRows[0];
                int enterpriseId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string enterpriseName = selectedRow.Cells["Название"].Value.ToString();

                string query = "DELETE FROM предприятия_розничной_торговли WHERE id_предприятия = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить предприятие: {enterpriseName}?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", enterpriseId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Предприятие удалено!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadEnterprisesData();
                            }
                            else
                            {
                                MessageBox.Show("Предприятие с указанным ID не найдено", "Ошибка",
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
                        id_предприятия as 'ID',
                        название as 'Название',
                        адрес as 'Адрес',
                        телефон as 'Телефон',
                        тип_предприятия as 'Тип',
                        email as 'Email'
                    FROM предприятия_розничной_торговли
                    WHERE название LIKE @SearchText + '%' OR 
                          адрес LIKE @SearchText + '%' OR
                          тип_предприятия LIKE @SearchText + '%' OR
                          телефон LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Предприятия не найдены!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridViewSupplies.DataSource = table;
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
                LoadEnterprisesData();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEnterprisesData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            cmbType.SelectedIndex = 0;
        }
    }
}
