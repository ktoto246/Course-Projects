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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Газрегион
{
    public partial class Form2 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=ГАЗРЕГИОН;Integrated Security=True;Encrypt=False";
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
                    string query = "SELECT * FROM отделы ";
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
                string query = "delete from отделы where id_отдела = @Value1";
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
                        string query = "SELECT * FROM отделы WHERE название_отдела LIKE @LastName + '%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@LastName", lastName);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Отдел не найден!");
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
                string query = "INSERT INTO отделы (название_отдела, описание_отдела, дата_создания) VALUES (@Value1,@Value2,@Value3)";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (txtDepartmentName.Text.Equals("") || txtDescription.Text.Equals(""))
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
                        command.Parameters.AddWithValue("@Value3", dateTimePicker1.Value);
                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные дабавлены!");
                            //Очистка полей
                            txtDepartmentName.Clear();
                            txtDescription.Clear();
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
