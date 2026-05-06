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
        private string connection = "Data Source=PC;Initial Catalog=КФХ_Горбов_НИ;Integrated Security=True;Encrypt=False";
        public Form3()
        {
            InitializeComponent();
            LoadCropsData();
        }
        private void LoadCropsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        id_культуры,
                        название_культуры,
                        тип_культуры,
                        срок_созревания_дни,
                        стоимость_посевного_материала,
                        урожайность_т_га
                    FROM сельхоз_культуры";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewCrops.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"INSERT INTO сельхоз_культуры 
                    (название_культуры, тип_культуры, срок_созревания_дни, стоимость_посевного_материала, урожайность_т_га) 
                    VALUES (@название, @тип, @срок, @стоимость, @урожайность)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(textBoxНазвание.Text) ||
                        string.IsNullOrEmpty(comboBoxТип.Text) ||
                        string.IsNullOrEmpty(textBoxСрок.Text))
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@название", textBoxНазвание.Text);
                        command.Parameters.AddWithValue("@тип", comboBoxТип.Text);
                        command.Parameters.AddWithValue("@срок", int.Parse(textBoxСрок.Text));
                        command.Parameters.AddWithValue("@стоимость", string.IsNullOrEmpty(textBoxСтоимость.Text) ? 0 : decimal.Parse(textBoxСтоимость.Text));
                        command.Parameters.AddWithValue("@урожайность", string.IsNullOrEmpty(textBoxУрожайность.Text) ? 0 : decimal.Parse(textBoxУрожайность.Text));

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            textBoxНазвание.Clear();
                            comboBoxТип.SelectedIndex = -1;
                            textBoxСрок.Clear();
                            textBoxСтоимость.Clear();
                            textBoxУрожайность.Clear();
                            LoadCropsData();
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "DELETE FROM сельхоз_культуры WHERE id_культуры = @id_культуры";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@id_культуры", textBoxID.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                textBoxID.Clear();
                                LoadCropsData();
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
                MessageBox.Show("Ошибка: Запись не найдена " + ex.Message);
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    try
                    {
                        conn.Open();
                        string query = @"SELECT 
                            id_культуры,
                            название_культуры,
                            тип_культуры,
                            срок_созревания_дни,
                            стоимость_посевного_материала,
                            урожайность_т_га
                        FROM сельхоз_культуры
                        WHERE название_культуры LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Культура не найдена!");
                        }

                        dataGridViewCrops.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message);
                    }
                }
            }
            else
            {
                LoadCropsData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadCropsData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
