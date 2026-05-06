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
        private string connection = "Data Source=PC;Initial Catalog=бд;Integrated Security=True;Encrypt=False";
        public Form4()
        {
            InitializeComponent();
            LoadProductionData();
            LoadComboBoxData();
        }
        private void LoadProductionData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        p.id_производства as 'ID',
                        t.название_товара as 'Товар',
                        e.фамилия + ' ' + e.имя as 'Сотрудник',
                        p.количество as 'Количество',
                        p.дата_производства as 'Дата производства',
                        p.статус as 'Статус'
                    FROM производство p
                    JOIN товары t ON p.id_товара = t.id_товара
                    JOIN сотрудники e ON p.id_сотрудника = e.id_сотрудника";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewProduction.DataSource = table;
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

                    // Загрузка товаров
                    string queryProducts = "SELECT id_товара, название_товара FROM товары";
                    SqlDataAdapter adapterProducts = new SqlDataAdapter(queryProducts, conn);
                    DataTable dtProducts = new DataTable();
                    adapterProducts.Fill(dtProducts);

                    cmbProduct.DataSource = dtProducts;
                    cmbProduct.DisplayMember = "название_товара";
                    cmbProduct.ValueMember = "id_товара";

                    // Загрузка сотрудников
                    string queryEmployees = "SELECT id_сотрудника, фамилия + ' ' + имя AS ФИО FROM сотрудники";
                    SqlDataAdapter adapterEmployees = new SqlDataAdapter(queryEmployees, conn);
                    DataTable dtEmployees = new DataTable();
                    adapterEmployees.Fill(dtEmployees);

                    cmbEmployee.DataSource = dtEmployees;
                    cmbEmployee.DisplayMember = "ФИО";
                    cmbEmployee.ValueMember = "id_сотрудника";

                    // Установка начального статуса
                    cmbStatus.SelectedIndex = 1; // "Завершено"
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
                string query = @"INSERT INTO производство 
                    (id_товара, id_сотрудника, количество, статус) 
                    VALUES (@Value1, @Value2, @Value3, @Value4)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (cmbProduct.SelectedValue == null || cmbEmployee.SelectedValue == null ||
                        string.IsNullOrEmpty(txtQuantity.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля: Товар, Сотрудник, Количество!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", cmbProduct.SelectedValue);
                        command.Parameters.AddWithValue("@Value2", cmbEmployee.SelectedValue);
                        command.Parameters.AddWithValue("@Value3", Convert.ToInt32(txtQuantity.Text));
                        command.Parameters.AddWithValue("@Value4", cmbStatus.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Запись производства добавлена!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadProductionData();
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
                if (string.IsNullOrEmpty(txtProductionId.Text))
                {
                    MessageBox.Show("Введите ID записи для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtProductionId.Text, out int productionId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "DELETE FROM производство WHERE id_производства = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить запись с ID: " + productionId + "?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", productionId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Запись удалена!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtProductionId.Clear();
                                LoadProductionData();
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

        private void btnRefresh_Click(object sender, EventArgs e)
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
                            p.id_производства as 'ID',
                            t.название_товара as 'Товар',
                            e.фамилия + ' ' + e.имя as 'Сотрудник',
                            p.количество as 'Количество',
                            p.дата_производства as 'Дата производства',
                            p.статус as 'Статус'
                        FROM производство p
                        JOIN товары t ON p.id_товара = t.id_товара
                        JOIN сотрудники e ON p.id_сотрудника = e.id_сотрудника
                        WHERE t.название_товара LIKE @SearchText + '%' OR 
                              e.фамилия LIKE @SearchText + '%' OR
                              p.статус LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Записи не найдены!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridViewProduction.DataSource = table;
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
                LoadProductionData();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadProductionData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtQuantity.Clear();
            cmbStatus.SelectedIndex = 1;
        }
    }
}
