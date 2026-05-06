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
    public partial class Form3 : Form
    {
        private string connection = "Data Source=PC;Initial Catalog=Магнит_Должности;Integrated Security=True;Encrypt=False";
        public Form3()
        {
            InitializeComponent();
            LoadPositionsData();
        }
        private void LoadPositionsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        д.id_должности,
                        о.название_отдела,
                        д.название_должности,
                        д.уровень_должности,
                        д.минимальная_зарплата,
                        д.максимальная_зарплата,
                        д.описание_обязанностей,
                        д.требования,
                        д.дата_создания
                    FROM Должности д
                    LEFT JOIN Отделы о ON д.id_отдела = о.id_отдела";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewPositions.DataSource = table;
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
                if (dataGridViewPositions.CurrentRow == null)
                {
                    MessageBox.Show("Выберите должность для удаления!");
                    return;
                }

                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту должность?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo);

                if (confirmResult == DialogResult.Yes)
                {
                    string query = "DELETE FROM Должности WHERE id_должности = @id_должности";

                    using (SqlConnection conn = new SqlConnection(connection))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            int positionId = Convert.ToInt32(dataGridViewPositions.CurrentRow.Cells["id_должности"].Value);
                            command.Parameters.AddWithValue("@id_должности", positionId);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                LoadPositionsData();
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
                            д.id_должности,
                            о.название_отдела,
                            д.название_должности,
                            д.уровень_должности,
                            д.минимальная_зарплата,
                            д.максимальная_зарплата,
                            д.описание_обязанностей,
                            д.требования,
                            д.дата_создания
                        FROM Должности д
                        LEFT JOIN Отделы о ON д.id_отдела = о.id_отдела
                        WHERE д.название_должности LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Должности не найдены!");
                        }

                        dataGridViewPositions.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message);
                    }
                }
            }
            else
            {
                LoadPositionsData();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LoadPositionsData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO Должности 
                    (id_отдела, название_должности, уровень_должности, минимальная_зарплата, 
                     максимальная_зарплата, описание_обязанностей, требования) 
                    VALUES (@id_отдела, @название_должности, @уровень_должности, @минимальная_зарплата, 
                            @максимальная_зарплата, @описание_обязанностей, @требования)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(txtPositionName.Text) || string.IsNullOrEmpty(txtMinSalary.Text) ||
                        string.IsNullOrEmpty(txtMaxSalary.Text) ||
                        cmbLevel.SelectedIndex == -1)
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@id_отдела", textBox1.Text);
                        command.Parameters.AddWithValue("@название_должности", txtPositionName.Text);
                        command.Parameters.AddWithValue("@уровень_должности", cmbLevel.Text);
                        command.Parameters.AddWithValue("@минимальная_зарплата", decimal.Parse(txtMinSalary.Text));
                        command.Parameters.AddWithValue("@максимальная_зарплата", decimal.Parse(txtMaxSalary.Text));
                        command.Parameters.AddWithValue("@описание_обязанностей", txtDescription.Text);
                        command.Parameters.AddWithValue("@требования", txtRequirements.Text);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            LoadPositionsData();
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
