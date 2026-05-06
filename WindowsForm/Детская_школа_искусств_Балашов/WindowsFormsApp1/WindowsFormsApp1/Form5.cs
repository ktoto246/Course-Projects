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
        private string connection = "Data Source=PC;Initial Catalog=Детская_школа_искусств_Балашов;Integrated Security=True;Encrypt=False";
        public Form5()
        {
            InitializeComponent();
            LoadScheduleData();
        }
        private void LoadScheduleData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    р.id_занятия,
                    CONCAT(п.фамилия, ' ', п.имя) AS преподаватель,
                    CONCAT(у.фамилия, ' ', у.имя) AS учащийся,
                    о.название_отделения,
                    р.день_недели,
                    р.время_начала,
                    р.время_окончания,
                    р.предмет,
                    р.кабинет
                FROM расписание р
                LEFT JOIN сотрудники п ON р.id_преподавателя = п.id_сотрудника
                LEFT JOIN учащиеся у ON р.id_учащегося = у.id_учащегося
                LEFT JOIN отделения о ON р.id_отделения = о.id_отделения";
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
                string query = @"INSERT INTO расписание 
                (id_преподавателя, id_учащегося, id_отделения, день_недели, время_начала, время_окончания, предмет, кабинет) 
                VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (txtDepartmentName.Text.Equals("") || textBox2.Text.Equals("") || textBox4.Text.Equals("") || textBox5.Text.Equals(""))
                    {
                        MessageBox.Show("Заполните все поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtDepartmentName.Text);
                        command.Parameters.AddWithValue("@Value2", textBox2.Text);
                        command.Parameters.AddWithValue("@Value3", textBox4.Text);
                        command.Parameters.AddWithValue("@Value4", comboBox1.Text);
                        command.Parameters.AddWithValue("@Value5", dateTimePicker1.Value.TimeOfDay);
                        command.Parameters.AddWithValue("@Value6", dateTimePicker2.Value.TimeOfDay);
                        command.Parameters.AddWithValue("@Value7", textBox5.Text);
                        command.Parameters.AddWithValue("@Value8", comboBox2.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            txtDepartmentName.Clear();
                            textBox2.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
                            comboBox1.Text = "";
                            comboBox2.Text = "";
                            LoadScheduleData();
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadScheduleData();
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
                        р.id_занятия,
                        CONCAT(п.фамилия, ' ', п.имя) AS преподаватель,
                        CONCAT(у.фамилия, ' ', у.имя) AS учащийся,
                        о.название_отделения,
                        р.день_недели,
                        р.время_начала,
                        р.время_окончания,
                        р.предмет,
                        р.кабинет
                    FROM расписание р
                    LEFT JOIN сотрудники п ON р.id_преподавателя = п.id_сотрудника
                    LEFT JOIN учащиеся у ON р.id_учащегося = у.id_учащегося
                    LEFT JOIN отделения о ON р.id_отделения = о.id_отделения
                    WHERE р.предмет LIKE @SearchText + '%' OR п.фамилия LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Занятие не найдено!");
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
                LoadScheduleData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "DELETE FROM расписание WHERE id_занятия = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult != DialogResult.Yes)
                        return;

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", textBox1.Text);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные удалены!");
                            textBox1.Clear();
                            LoadScheduleData();
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
                MessageBox.Show("Ошибка: Запись не найдена " + ex.Message);
            }
        }
    }
}
