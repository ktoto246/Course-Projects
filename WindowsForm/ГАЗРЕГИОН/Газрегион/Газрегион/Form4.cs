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

namespace Газрегион
{
    public partial class Form4 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=ГАЗРЕГИОН;Integrated Security=True;Encrypt=False";
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
    п.id_проекта,
    п.название_проекта,
    п.описание_проекта,
    CONCAT(р.фамилия, ' ', р.имя, ' ', COALESCE(р.отчество, '')) AS руководитель_фио,
    п.дата_начала,
    п.дата_окончания,
    п.статус_проекта,
    п.бюджет
FROM проекты п
LEFT JOIN сотрудники р ON п.id_руководителя = р.id_сотрудника";
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
        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //Запрос
                string query = "delete from проекты where id_проекта = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?");

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", textBox1.Text);
                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные удалены!");
                            //Очистка полей
                            textBox1.Clear();
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

        private void btnSave_Click(object sender, EventArgs e)
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
    п.id_проекта,
    п.название_проекта,
    п.описание_проекта,
    CONCAT(р.фамилия, ' ', р.имя, ' ', COALESCE(р.отчество, '')) AS руководитель_фио,
    п.дата_начала,
    п.дата_окончания,
    п.статус_проекта,
    п.бюджет
FROM проекты п
LEFT JOIN сотрудники р ON п.id_руководителя = р.id_сотрудника
WHERE  п.название_проекта LIKE @LastName + '%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@LastName", lastName);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Проект не найден!");
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProductsData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {

                //Запрос
                string query = "INSERT INTO проекты (название_проекта, описание_проекта, id_руководителя, дата_начала, дата_окончания, статус_проекта, бюджет) VALUES (@Value1,@Value2,@Value3,@Value4,@Value5,@Value6,@Value7)";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (txtDepartmentName.Text.Equals("") || txtDescription.Text.Equals("") || comboBox1.Text.Equals("") || textBox8.Text.Equals(""))
                    {
                        MessageBox.Show("Заполните все поля!");
                        return;
                    }
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        //Добавление
                        command.Parameters.AddWithValue("@Value1", txtDepartmentName.Text);
                        command.Parameters.AddWithValue("@Value2", txtDescription.Text);
                        command.Parameters.AddWithValue("@Value3", textBox2.Text);
                        command.Parameters.AddWithValue("@Value4", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@Value5", dateTimePicker2.Value);
                        command.Parameters.AddWithValue("@Value6", comboBox1.Text);
                        command.Parameters.AddWithValue("@Value7", textBox8.Text);
                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные дабавлены!");
                            //Очистка полей
                            txtDepartmentName.Clear();
                            txtDescription.Clear();
                            textBox2.Clear();
                            textBox8.Clear();
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
    }
}
