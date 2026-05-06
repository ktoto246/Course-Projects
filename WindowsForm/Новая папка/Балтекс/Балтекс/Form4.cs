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

namespace Балтекс
{
    public partial class Form4 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Балтекс;Integrated Security=True;Encrypt=False";
        public Form4()
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
    з.Код_Заказа,
    з.Номер as НомерЗаказа,
    к.Название as Клиент,
    п.Наименование as Продукция,
    з.Дата,
    з.Статус,
    з.Количество,
    з.Цена,
    з.Общая_сумма,
    з.Примечание
FROM Заказы з
INNER JOIN Клиенты к ON з.Код_клиента = к.Код_Клиента
INNER JOIN Продукция п ON з.Код_продукта = п.Код_Продукта";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                //Запрос
                string query = "INSERT INTO Заказы (Номер, Код_клиента, Код_продукта, Дата, Статус, Количество, Цена, Общая_сумма, Примечание) VALUES (@Value1,@Value2,@Value3,@Value4,@Value5,@Value6,@Value7,@Value8,@Value9)";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (textBox1.Text.Equals("") || textBox2.Text.Equals("") || textBox3.Text.Equals("")  || textBox5.Text.Equals("") || textBox6.Text.Equals("") || textBox7.Text.Equals("") || textBox8.Text.Equals("") || textBox9.Text.Equals(""))
                    {
                        MessageBox.Show("Заполните все поля!");
                        return;
                    }
                    conn.Open();

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        //Добавление
                        command.Parameters.AddWithValue("@Value1", textBox1.Text);
                        command.Parameters.AddWithValue("@Value2", textBox2.Text);
                        command.Parameters.AddWithValue("@Value3", textBox3.Text);
                        command.Parameters.AddWithValue("@Value4", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@Value5", textBox5.Text);
                        command.Parameters.AddWithValue("@Value6", textBox6.Text);
                        command.Parameters.AddWithValue("@Value7", textBox7.Text);
                        command.Parameters.AddWithValue("@Value8", textBox8.Text);
                        command.Parameters.AddWithValue("@Value9", textBox9.Text);
                        

                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные дабавлены!");
                            //Очистка полей
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                          
                            textBox5.Clear();
                            textBox6.Clear();
                            textBox7.Clear();
                            textBox8.Clear();
                            textBox9.Clear();
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Запрос
                string query = "delete from Заказы where Код_Заказа = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?");

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", textBox10.Text);
                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные удалены!");
                            //Очистка полей
                            textBox10.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении данных");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:Запись не найдена " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string lastName = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(lastName))
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        conn.Open();
                        string query = @"SELECT 
    з.Код_Заказа,
    з.Номер as НомерЗаказа,
    к.Название as Клиент,
    п.Наименование as Продукция,
    з.Дата,
    з.Статус,
    з.Количество,
    з.Цена,
    з.Общая_сумма,
    з.Примечание
FROM Заказы з
INNER JOIN Клиенты к ON з.Код_клиента = к.Код_Клиента
INNER JOIN Продукция п ON з.Код_продукта = п.Код_Продукта
WHERE з.Статус LIKE @LastName + '%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@LastName", lastName);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Продукция не найдена!");
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
                LoadProductsData();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadProductsData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
        private void FormOrders_Load(object sender, EventArgs e)
        {
            // Устанавливаем начальные значения
            comboBoxSortField.SelectedIndex = 0;
            radioAsc.Checked = true;

            // Загружаем данные
            LoadOrdersData();
        }
        private void LoadOrdersData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
    з.Код_Заказа,
    з.Номер as НомерЗаказа,
    к.Название as Клиент,
    п.Наименование as Продукция,
    з.Дата,
    з.Статус,
    з.Количество,
    з.Цена,
    з.Общая_сумма,
    з.Примечание
FROM Заказы з
INNER JOIN Клиенты к ON з.Код_клиента = к.Код_Клиента
INNER JOIN Продукция п ON з.Код_продукта = п.Код_Продукта";

                    // Добавляем сортировку если выбраны параметры
                    if (comboBoxSortField.SelectedItem != null)
                    {
                        string sortField = GetSortField();
                        string sortDirection = GetSortDirection();
                        query += " ORDER BY " + sortField + " " + sortDirection;
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
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

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadOrdersData();
        }
        private string GetSortField()
        {
            if (comboBoxSortField.SelectedItem != null)
            {
                string selectedField = comboBoxSortField.SelectedItem.ToString();

                if (selectedField == "Номер заказа")
                    return "з.Номер";
                else if (selectedField == "Клиент")
                    return "к.Название";
                else if (selectedField == "Продукция")
                    return "п.Наименование";
                else if (selectedField == "Дата")
                    return "з.Дата";
                else if (selectedField == "Статус")
                    return "з.Статус";
                else if (selectedField == "Количество")
                    return "з.Количество";
                else if (selectedField == "Цена")
                    return "з.Цена";
                else if (selectedField == "Общая сумма")
                    return "з.Общая_сумма";
            }
            return "з.Номер"; // Значение по умолчанию
        }

        private string GetSortDirection()
        {
            if (radioAsc.Checked)
                return "ASC";
            else
                return "DESC";
        }
    }
}
