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

namespace Магнит
{
    public partial class Form4 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Магнит_Должности;Integrated Security=True;Encrypt=False";
        public Form4()
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
                        id_отдела,
                        название_отдела,
                        описание_отдела,
                        дата_создания
                    FROM Отделы";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewDepartments.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            LoadDepartmentsData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
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
                            id_отдела,
                            название_отдела,
                            описание_отдела,
                            дата_создания
                        FROM Отделы
                        WHERE название_отдела LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Отделы не найдены!");
                        }

                        dataGridViewDepartments.DataSource = table;
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
                if (dataGridViewDepartments.CurrentRow == null)
                {
                    MessageBox.Show("Выберите отдел для удаления!");
                    return;
                }

                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить этот отдел?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    string query = "DELETE FROM Отделы WHERE id_отдела = @id_отдела";

                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            int departmentId = Convert.ToInt32(dataGridViewDepartments.CurrentRow.Cells["id_отдела"].Value);
                            command.Parameters.AddWithValue("@id_отдела", departmentId);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                LoadDepartmentsData();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при удалении данных");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO Отделы 
                    (название_отдела, описание_отдела) 
                    VALUES (@название_отдела, @описание_отдела)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtDepartmentName.Text))
                    {
                        MessageBox.Show("Заполните название отдела!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@название_отдела", txtDepartmentName.Text);
                        command.Parameters.AddWithValue("@описание_отдела", txtDescription.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
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

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}
