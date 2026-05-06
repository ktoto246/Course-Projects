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
        private string connection = "Data Source=PC;Initial Catalog=Детская_школа_искусств_Балашов;Integrated Security=True;Encrypt=False";
        public Form4()
        {
            InitializeComponent();
            LoadStudentsData();
        }
        private void LoadStudentsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    у.id_учащегося,
                    у.фамилия,
                    у.имя,
                    у.отчество,
                    у.дата_рождения,
                    у.класс,
                    о.название_отделения,
                    CONCAT(п.фамилия, ' ', п.имя, ' ', COALESCE(п.отчество, '')) AS преподаватель,
                    у.год_поступления
                FROM учащиеся у
                LEFT JOIN отделения о ON у.id_отделения = о.id_отделения
                LEFT JOIN сотрудники п ON у.id_преподавателя = п.id_сотрудника";
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
                string query = @"INSERT INTO учащиеся 
                (фамилия, имя, отчество, дата_рождения, класс, id_отделения, id_преподавателя, год_поступления) 
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
                        command.Parameters.AddWithValue("@Value4", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@Value5", textBox5.Text);
                        command.Parameters.AddWithValue("@Value6", comboBox1.Text);
                        command.Parameters.AddWithValue("@Value7", comboBox2.Text);
                        command.Parameters.AddWithValue("@Value8", dateTimePicker2.Value.Year);

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
                            LoadStudentsData();
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
                string query = "DELETE FROM учащиеся WHERE id_учащегося = @Value1";
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
                            LoadStudentsData();
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
                        у.id_учащегося,
                        у.фамилия,
                        у.имя,
                        у.отчество,
                        у.дата_рождения,
                        у.класс,
                        о.название_отделения,
                        CONCAT(п.фамилия, ' ', п.имя, ' ', COALESCE(п.отчество, '')) AS преподаватель,
                        у.год_поступления
                    FROM учащиеся у
                    LEFT JOIN отделения о ON у.id_отделения = о.id_отделения
                    LEFT JOIN сотрудники п ON у.id_преподавателя = п.id_сотрудника
                    WHERE у.фамилия LIKE @SearchText + '%' OR у.имя LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Учащийся не найден!");
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
                LoadStudentsData();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStudentsData();
        }
    }
}
