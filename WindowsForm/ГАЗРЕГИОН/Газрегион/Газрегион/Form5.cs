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
    public partial class Form5 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=ГАЗРЕГИОН;Integrated Security=True;Encrypt=False";
        public Form5()
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
    з.id_задачи,
п.название_проекта,
CONCAT(и.фамилия, ' ', и.имя, ' ', COALESCE(и.отчество, '')) AS исполнитель_фио,
    з.название_задачи,
    з.описание_задачи,
    и.должность AS должность_исполнителя,
    з.приоритет,
    з.статус_задачи,
    з.дата_создания,
    з.срок_выполнения,
    з.дата_завершения
FROM задачи з
LEFT JOIN проекты п ON з.id_проекта = п.id_проекта
LEFT JOIN сотрудники и ON з.id_исполнителя = и.id_сотрудника";
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                //Запрос
                string query = "INSERT INTO задачи (id_проекта, id_исполнителя, название_задачи, описание_задачи, приоритет, статус_задачи, дата_создания, срок_выполнения, дата_завершения) VALUES (@Value1,@Value2,@Value3,@Value4,@Value5,@Value6,@Value7,@Value8,@Value9)";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (txtDepartmentName.Text.Equals("") || textBox2.Text.Equals("") || textBox5.Text.Equals("") || textBox4.Text.Equals("") || comboBox1.Text.Equals("") || comboBox2.Text.Equals(""))
                    {
                        MessageBox.Show("Заполните все поля!");
                        return;
                    }
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        //Добавление
                        command.Parameters.AddWithValue("@Value1", txtDepartmentName.Text);
                        command.Parameters.AddWithValue("@Value2", textBox2.Text);
                        command.Parameters.AddWithValue("@Value3", textBox5.Text);
                        command.Parameters.AddWithValue("@Value4", textBox4.Text);
                        command.Parameters.AddWithValue("@Value5", comboBox1.Text);
                        command.Parameters.AddWithValue("@Value6", comboBox2.Text);
                        command.Parameters.AddWithValue("@Value7", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@Value8", dateTimePicker2.Value);
                        command.Parameters.AddWithValue("@Value9", dateTimePicker3.Value);
                        //Выполнение
                        int result = command.ExecuteNonQuery();
                        //Проверка
                        if (result > 0)
                        {
                            MessageBox.Show("Данные дабавлены!");
                            //Очистка полей
                            txtDepartmentName.Clear();
                            textBox2.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //Запрос
                string query = "delete from задачи where id_задачи = @Value1";
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
    з.id_задачи,
п.название_проекта,
CONCAT(и.фамилия, ' ', и.имя, ' ', COALESCE(и.отчество, '')) AS исполнитель_фио,
    з.название_задачи,
    з.описание_задачи,
    и.должность AS должность_исполнителя,
    з.приоритет,
    з.статус_задачи,
    з.дата_создания,
    з.срок_выполнения,
    з.дата_завершения
FROM задачи з
LEFT JOIN проекты п ON з.id_проекта = п.id_проекта
LEFT JOIN сотрудники и ON з.id_исполнителя = и.id_сотрудника
WHERE з.название_задачи LIKE @LastName + '%'";
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
    }
}
