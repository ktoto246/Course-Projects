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
        private string connection = "Data Source=PC;Initial Catalog=бд;Integrated Security=True;Encrypt=False";
        public Form5()
        {
            InitializeComponent();
            LoadSalesData();
            LoadComboBoxData();
        }
        private void LoadSalesData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        s.id_продажи as 'ID',
                        t.название_товара as 'Товар',
                        e.фамилия + ' ' + e.имя as 'Сотрудник',
                        s.количество as 'Количество',
                        s.сумма as 'Сумма',
                        s.дата_продажи as 'Дата продажи'
                    FROM продажи s
                    JOIN товары t ON s.id_товара = t.id_товара
                    JOIN сотрудники e ON s.id_сотрудника = e.id_сотрудника";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewSales.DataSource = table;
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
                    string queryProducts = "SELECT id_товара, название_товара, цена_продажи FROM товары";
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
                string query = @"INSERT INTO продажи 
                    (id_товара, id_сотрудника, количество, сумма) 
                    VALUES (@Value1, @Value2, @Value3, @Value4)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (cmbProduct.SelectedValue == null || cmbEmployee.SelectedValue == null ||
                        string.IsNullOrEmpty(txtQuantity.Text) || string.IsNullOrEmpty(txtAmount.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля: Товар, Сотрудник, Количество, Сумма!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", cmbProduct.SelectedValue);
                        command.Parameters.AddWithValue("@Value2", cmbEmployee.SelectedValue);
                        command.Parameters.AddWithValue("@Value3", Convert.ToInt32(txtQuantity.Text));
                        command.Parameters.AddWithValue("@Value4", Convert.ToDecimal(txtAmount.Text));

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Продажа добавлена!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadSalesData();
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
                if (string.IsNullOrEmpty(txtSaleId.Text))
                {
                    MessageBox.Show("Введите ID продажи для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtSaleId.Text, out int saleId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "DELETE FROM продажи WHERE id_продажи = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить продажу с ID: " + saleId + "?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", saleId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Продажа удалена!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtSaleId.Clear();
                                LoadSalesData();
                            }
                            else
                            {
                                MessageBox.Show("Продажа с указанным ID не найдена", "Ошибка",
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
                            s.id_продажи as 'ID',
                            t.название_товара as 'Товар',
                            e.фамилия + ' ' + e.имя as 'Сотрудник',
                            s.количество as 'Количество',
                            s.сумма as 'Сумма',
                            s.дата_продажи as 'Дата продажи'
                        FROM продажи s
                        JOIN товары t ON s.id_товара = t.id_товара
                        JOIN сотрудники e ON s.id_сотрудника = e.id_сотрудника
                        WHERE t.название_товара LIKE @SearchText + '%' OR 
                              e.фамилия LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Продажи не найдены!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridViewSales.DataSource = table;
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
                LoadSalesData();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadSalesData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtQuantity.Clear();
            txtAmount.Clear();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        private void CalculateAmount()
        {
            if (cmbProduct.SelectedValue != null && !string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                if (int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connection))
                        {
                            conn.Open();
                            string query = "SELECT цена_продажи FROM товары WHERE id_товара = @ProductId";
                            using (SqlCommand command = new SqlCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@ProductId", cmbProduct.SelectedValue);
                                object result = command.ExecuteScalar();

                                if (result != null && decimal.TryParse(result.ToString(), out decimal price))
                                {
                                    decimal amount = quantity * price;
                                    txtAmount.Text = amount.ToString("F2");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при расчете суммы: " + ex.Message, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
