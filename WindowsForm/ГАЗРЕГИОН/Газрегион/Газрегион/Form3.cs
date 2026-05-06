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
    public partial class Form3 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=ГАЗРЕГИОН;Integrated Security=True;Encrypt=False";
        public Form3()
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
    с.id_сотрудника,
   о.название_отдела,
с.табельный_номер,
    с.фамилия,
    с.имя,
    с.отчество,
    с.должность,
    с.телефон,
    с.email,
    с.дата_приема,
    с.статус
FROM сотрудники с
LEFT JOIN отделы о ON с.id_отдела = о.id_отдела";
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
                string query = "delete from сотрудники where id_сотрудника = @Value1";
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
    с.id_сотрудника,
   о.название_отдела,
с.табельный_номер,
    с.фамилия,
    с.имя,
    с.отчество,
    с.должность,
    с.телефон,
    с.email,
    с.дата_приема,
    с.статус
FROM сотрудники с
LEFT JOIN отделы о ON с.id_отдела = о.id_отдела 
WHERE с.фамилия LIKE @LastName + '%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@LastName", lastName);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Сотрудник не найден!");
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                //Запрос
                string query = "INSERT INTO сотрудники (id_отдела, табельный_номер, фамилия, имя, отчество, должность, телефон, email, дата_приема, статус) VALUES (@Value1,@Value2,@Value3,@Value4,@Value5,@Value6,@Value7,@Value8,@Value9,@Value10)";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (txtDepartmentName.Text.Equals("") || txtDescription.Text.Equals("") || textBox2.Text.Equals("") || textBox5.Text.Equals("") || textBox4.Text.Equals("") || textBox3.Text.Equals("") || textBox8.Text.Equals("") || textBox7.Text.Equals("") || comboBox1.Text.Equals(""))
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
                        command.Parameters.AddWithValue("@Value4", textBox5.Text);
                        command.Parameters.AddWithValue("@Value5", textBox4.Text);
                        command.Parameters.AddWithValue("@Value6", textBox3.Text);
                        command.Parameters.AddWithValue("@Value7", textBox8.Text);
                        command.Parameters.AddWithValue("@Value8", textBox7.Text);
                        command.Parameters.AddWithValue("@Value9", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@Value10", comboBox1.Text);
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
                            textBox5.Clear();
                            textBox4.Clear();
                            textBox3.Clear();
                            textBox8.Clear();
                            textBox7.Clear();
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
