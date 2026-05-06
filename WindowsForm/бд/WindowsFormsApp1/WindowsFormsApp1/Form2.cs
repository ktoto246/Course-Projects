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
        private string connection = "Data Source=PC;Initial Catalog=бд;Integrated Security=True;Encrypt=False";
        public Form2()
        {
            InitializeComponent();
            LoadProductsData();
        }
        private void LoadProductsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        id_товара as 'ID',
                        название_товара as 'Название',
                        категория as 'Категория',
                        вес_граммы as 'Вес (г)',
                        срок_годности_часы as 'Срок годности (ч)',
                        цена_производства as 'Цена производства',
                        цена_продажи as 'Цена продажи'
                    FROM товары";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewProducts.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO товары 
                    (название_товара, категория, вес_граммы, срок_годности_часы, цена_производства, цена_продажи) 
                    VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtProductName.Text) || string.IsNullOrEmpty(comboBox1.Text) ||
                        string.IsNullOrEmpty(txtWeight.Text) || string.IsNullOrEmpty(txtShelfLife.Text) ||
                        string.IsNullOrEmpty(txtProductionPrice.Text) || string.IsNullOrEmpty(txtSalePrice.Text))
                    {
                        MessageBox.Show("Заполните все поля!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtProductName.Text);
                        command.Parameters.AddWithValue("@Value2", comboBox1.Text);
                        command.Parameters.AddWithValue("@Value3", Convert.ToInt32(txtWeight.Text));
                        command.Parameters.AddWithValue("@Value4", Convert.ToInt32(txtShelfLife.Text));
                        command.Parameters.AddWithValue("@Value5", Convert.ToDecimal(txtProductionPrice.Text));
                        command.Parameters.AddWithValue("@Value6", Convert.ToDecimal(txtSalePrice.Text));

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Товар добавлен!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadProductsData();
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
                if (string.IsNullOrEmpty(txtProductId.Text))
                {
                    MessageBox.Show("Введите ID товара для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtProductId.Text, out int productId))
                {
                    MessageBox.Show("ID должен быть числом!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "DELETE FROM товары WHERE id_товара = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить товар с ID: " + productId + "?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", productId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Товар удален!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtProductId.Clear();
                                LoadProductsData();
                            }
                            else
                            {
                                MessageBox.Show("Товар с указанным ID не найден", "Ошибка",
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
                            id_товара as 'ID',
                            название_товара as 'Название',
                            категория as 'Категория',
                            вес_граммы as 'Вес (г)',
                            срок_годности_часы as 'Срок годности (ч)',
                            цена_производства as 'Цена производства',
                            цена_продажи as 'Цена продажи'
                        FROM товары
                        WHERE название_товара LIKE @SearchText + '%' OR 
                              категория LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Товары не найдены!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridViewProducts.DataSource = table;
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
                LoadProductsData();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LoadProductsData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtProductName.Clear();
            txtWeight.Clear();
            txtShelfLife.Clear();
            txtProductionPrice.Clear();
            txtSalePrice.Clear();
        }
    }
}
