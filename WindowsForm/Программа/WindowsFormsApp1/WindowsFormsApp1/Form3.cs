using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=БАЛКОМХЛЕБПРОДУКТ;Integrated Security=True;Encrypt=False";
        public Form3()
        {
            InitializeComponent();
            LoadProductsData();
            LoadComboBoxData();
        }
        private void LoadProductsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    id_продукта as 'ID',
                    наименование as 'Наименование',
                    категория as 'Категория',
                    единица_измерения as 'Единица',
                    цена_за_единицу as 'Цена',
                    срок_годности_дни as 'Срок годности',
                    описание as 'Описание'
                FROM продукты";
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
                cmbCategory.Items.AddRange(new string[] {
                "хлеб", "булочные изделия", "кондитерские изделия", "мука", "крупы", "прочее"
            });

                cmbUnit.Items.AddRange(new string[] {
                "кг", "г", "шт", "уп", "пак", "банка"
            });

                cmbCategory.SelectedIndex = 0;
                cmbUnit.SelectedIndex = 0;
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
                string query = @"INSERT INTO продукты 
                (наименование, категория, единица_измерения, цена_за_единицу, срок_годности_дни, описание) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtName.Text) || cmbCategory.SelectedItem == null ||
                        cmbUnit.SelectedItem == null || string.IsNullOrEmpty(txtPrice.Text) ||
                        string.IsNullOrEmpty(txtShelfLife.Text))
                    {
                        MessageBox.Show("Заполните обязательные поля!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                    {
                        MessageBox.Show("Цена должна быть положительным числом!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!int.TryParse(txtShelfLife.Text, out int shelfLife) || shelfLife <= 0)
                    {
                        MessageBox.Show("Срок годности должен быть положительным числом!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtName.Text);
                        command.Parameters.AddWithValue("@Value2", cmbCategory.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Value3", cmbUnit.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Value4", price);
                        command.Parameters.AddWithValue("@Value5", shelfLife);
                        command.Parameters.AddWithValue("@Value6", txtDescription.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Продукт добавлен!", "Успех",
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
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите продукт для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int productId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string productName = selectedRow.Cells["Наименование"].Value.ToString();

                string query = "DELETE FROM продукты WHERE id_продукта = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить продукт: {productName}?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", productId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Продукт удален!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadProductsData();
                            }
                            else
                            {
                                MessageBox.Show("Продукт с указанным ID не найден", "Ошибка",
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
                        id_продукта as 'ID',
                        наименование as 'Наименование',
                        категория as 'Категория',
                        единица_измерения as 'Единица',
                        цена_за_единицу as 'Цена',
                        срок_годности_дни as 'Срок годности',
                        описание as 'Описание'
                    FROM продукты
                    WHERE наименование LIKE @SearchText + '%' OR 
                          категория LIKE @SearchText + '%' OR
                          описание LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Продукты не найдены!", "Информация",
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
                LoadProductsData();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProductsData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtName.Clear();
            txtPrice.Clear();
            txtShelfLife.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = 0;
            cmbUnit.SelectedIndex = 0;
        }
    }
}
