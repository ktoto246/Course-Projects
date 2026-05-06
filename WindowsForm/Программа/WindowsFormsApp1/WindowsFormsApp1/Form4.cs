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
        private string connection = "Data Source=PC;Initial Catalog=БАЛКОМХЛЕБПРОДУКТ;Integrated Security=True;Encrypt=False";
        public Form4()
        {
            InitializeComponent();
            LoadSuppliesData();
            LoadComboBoxData();
            CalculateTotal();
        }
        private void CalculateTotal()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtQuantity.Text) && !string.IsNullOrEmpty(txtPrice.Text))
                {
                    if (decimal.TryParse(txtQuantity.Text, out decimal quantity) &&
                        decimal.TryParse(txtPrice.Text, out decimal price))
                    {
                        decimal total = quantity * price;
                        txtTotal.Text = total.ToString("F2");
                    }
                }
            }
            catch (Exception)
            {
                txtTotal.Text = "0.00";
            }
        }
        private void LoadSuppliesData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    п.id_поставки as 'ID',
                    пр.название as 'Предприятие',
                    с.фамилия + ' ' + с.имя as 'Ответственный',
                    прод.наименование as 'Продукт',
                    п.количество as 'Количество',
                    прод.единица_измерения as 'Единица',
                    п.цена_на_момент_поставки as 'Цена',
                    п.сумма_поставки as 'Сумма',
                    п.дата_поставки as 'Дата поставки',
                    п.статус_поставки as 'Статус',
                    п.номер_накладной as 'Накладная'
                FROM поставки п
                JOIN предприятия_розничной_торговли пр ON п.id_предприятия = пр.id_предприятия
                JOIN сотрудники с ON п.id_сотрудника_ответственного = с.id_сотрудника
                JOIN продукты прод ON п.id_продукта = прод.id_продукта";
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

                    // Загрузка предприятий
                    string queryEnterprises = "SELECT id_предприятия, название FROM предприятия_розничной_торговли";
                    SqlDataAdapter adapterEnterprises = new SqlDataAdapter(queryEnterprises, conn);
                    DataTable dtEnterprises = new DataTable();
                    adapterEnterprises.Fill(dtEnterprises);

                    cmbEnterprise.DataSource = dtEnterprises;
                    cmbEnterprise.DisplayMember = "название";
                    cmbEnterprise.ValueMember = "id_предприятия";

                    // Загрузка сотрудников
                    string queryEmployees = "SELECT id_сотрудника, фамилия + ' ' + имя AS ФИО FROM сотрудники WHERE должность IN ('менеджер по продажам', 'логист', 'директор')";
                    SqlDataAdapter adapterEmployees = new SqlDataAdapter(queryEmployees, conn);
                    DataTable dtEmployees = new DataTable();
                    adapterEmployees.Fill(dtEmployees);

                    cmbEmployee.DataSource = dtEmployees;
                    cmbEmployee.DisplayMember = "ФИО";
                    cmbEmployee.ValueMember = "id_сотрудника";

                    // Загрузка продуктов
                    string queryProducts = "SELECT id_продукта, наименование FROM продукты";
                    SqlDataAdapter adapterProducts = new SqlDataAdapter(queryProducts, conn);
                    DataTable dtProducts = new DataTable();
                    adapterProducts.Fill(dtProducts);

                    cmbProduct.DataSource = dtProducts;
                    cmbProduct.DisplayMember = "наименование";
                    cmbProduct.ValueMember = "id_продукта";

                    // Установка статусов
                    cmbStatus.Items.AddRange(new string[] {
                    "в обработке", "подтверждена", "в пути", "доставлена", "отменена"
                });
                    cmbStatus.SelectedIndex = 0;

                    // Установка текущей даты
                    dtpSupplyDate.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных для выпадающих списков: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO поставки 
                (id_предприятия, id_сотрудника_ответственного, id_продукта, количество, 
                 цена_на_момент_поставки, сумма_поставки, дата_поставки, статус_поставки, номер_накладной) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (cmbEnterprise.SelectedValue == null || cmbEmployee.SelectedValue == null ||
                        cmbProduct.SelectedValue == null || string.IsNullOrEmpty(txtQuantity.Text) ||
                        string.IsNullOrEmpty(txtPrice.Text) || cmbStatus.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните обязательные поля!", "Внимание",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
                    {
                        MessageBox.Show("Количество должно быть положительным числом!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                    {
                        MessageBox.Show("Цена должна быть положительным числом!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    decimal total = quantity * price;

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", cmbEnterprise.SelectedValue);
                        command.Parameters.AddWithValue("@Value2", cmbEmployee.SelectedValue);
                        command.Parameters.AddWithValue("@Value3", cmbProduct.SelectedValue);
                        command.Parameters.AddWithValue("@Value4", quantity);
                        command.Parameters.AddWithValue("@Value5", price);
                        command.Parameters.AddWithValue("@Value6", total);
                        command.Parameters.AddWithValue("@Value7", dtpSupplyDate.Value);
                        command.Parameters.AddWithValue("@Value8", cmbStatus.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Value9", txtInvoice.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Поставка добавлена!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadSuppliesData();
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
                    MessageBox.Show("Выберите поставку для удаления!", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int supplyId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string enterpriseName = selectedRow.Cells["Предприятие"].Value.ToString();
                string productName = selectedRow.Cells["Продукт"].Value.ToString();

                string query = "DELETE FROM поставки WHERE id_поставки = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить поставку:\nПредприятие: {enterpriseName}\nПродукт: {productName}?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@Value1", supplyId);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Поставка удалена!", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadSuppliesData();
                            }
                            else
                            {
                                MessageBox.Show("Поставка с указанным ID не найдена", "Ошибка",
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

        private void btnSave_Click_1(object sender, EventArgs e)
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
                        п.id_поставки as 'ID',
                        пр.название as 'Предприятие',
                        с.фамилия + ' ' + с.имя as 'Ответственный',
                        прод.наименование as 'Продукт',
                        п.количество as 'Количество',
                        прод.единица_измерения as 'Единица',
                        п.цена_на_момент_поставки as 'Цена',
                        п.сумма_поставки as 'Сумма',
                        п.дата_поставки as 'Дата поставки',
                        п.статус_поставки as 'Статус',
                        п.номер_накладной as 'Накладная'
                    FROM поставки п
                    JOIN предприятия_розничной_торговли пр ON п.id_предприятия = пр.id_предприятия
                    JOIN сотрудники с ON п.id_сотрудника_ответственного = с.id_сотрудника
                    JOIN продукты прод ON п.id_продукта = прод.id_продукта
                    WHERE пр.название LIKE @SearchText + '%' OR 
                          с.фамилия LIKE @SearchText + '%' OR
                          прод.наименование LIKE @SearchText + '%' OR
                          п.номер_накладной LIKE @SearchText + '%' OR
                          п.статус_поставки LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Поставки не найдены!", "Информация",
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
                LoadSuppliesData();
            }
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            LoadSuppliesData();
            txtSearch.Clear();
        }
        private void ClearFields()
        {
            txtQuantity.Clear();
            txtPrice.Clear();
            txtTotal.Clear();
            txtInvoice.Clear();
            cmbStatus.SelectedIndex = 0;
            dtpSupplyDate.Value = DateTime.Now;

            if (cmbEnterprise.Items.Count > 0) cmbEnterprise.SelectedIndex = 0;
            if (cmbEmployee.Items.Count > 0) cmbEmployee.SelectedIndex = 0;
            if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
        }
    }
}
