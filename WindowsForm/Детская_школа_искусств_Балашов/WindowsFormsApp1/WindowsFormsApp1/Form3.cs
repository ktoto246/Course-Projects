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
    public partial class Form3 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Детская_школа_искусств_Балашов;Integrated Security=True;Encrypt=False";
        public Form3()
        {
            InitializeComponent();
            LoadDepartmentsData();
        }
        private void LoadDepartmentsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                    о.id_отделения,
                    о.название_отделения,
                    CONCAT(с.фамилия, ' ', с.имя, ' ', COALESCE(с.отчество, '')) AS заведующий,
                    о.этаж,
                    о.кабинет
                FROM отделения о
                LEFT JOIN сотрудники с ON о.заведующий_отделением = с.id_сотрудника";
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
                string query = @"INSERT INTO отделения 
                (название_отделения, заведующий_отделением, этаж, кабинет) 
                VALUES (@Value1, @Value2, @Value3, @Value4)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", txtDepartmentName.Text);
                        command.Parameters.AddWithValue("@Value2", textBox2.Text);
                        command.Parameters.AddWithValue("@Value3", textBox4.Text);
                        command.Parameters.AddWithValue("@Value4", textBox5.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            txtDepartmentName.Clear();
                            textBox2.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
                            LoadDepartmentsData();
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
            LoadDepartmentsData();
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
                        о.id_отделения,
                        о.название_отделения,
                        CONCAT(с.фамилия, ' ', с.имя, ' ', COALESCE(с.отчество, '')) AS заведующий,
                        о.этаж,
                        о.кабинет
                    FROM отделения о
                    LEFT JOIN сотрудники с ON о.заведующий_отделением = с.id_сотрудника
                    WHERE о.название_отделения LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Отделение не найдено!");
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
                LoadDepartmentsData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "DELETE FROM отделения WHERE id_отделения = @Value1";
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult != DialogResult.Yes)
                        return;

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Value1", textBox3.Text);
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные удалены!");
                            textBox3.Clear();
                            LoadDepartmentsData();
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
