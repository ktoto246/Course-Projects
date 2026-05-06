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
        private string connection = "Data Source=PC;Initial Catalog=КФХ_Горбов_НИ;Integrated Security=True;Encrypt=False";
        public Form5()
        {
            InitializeComponent();
            LoadFinanceData();
        }
        private void LoadFinanceData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        ф.id_операции,
                        ф.дата_операции,
                        ф.тип_операции,
                        ф.сумма,
                        ф.категория,
                        ф.описание,
                        CONCAT(с.фамилия, ' ', с.имя) AS сотрудник,
                        у.название_участка
                    FROM финансовые_операции ф
                    LEFT JOIN сотрудники с ON ф.id_сотрудника = с.id_сотрудника
                    LEFT JOIN участки у ON ф.id_участка = у.id_участка";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewFinance.DataSource = table;
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
                string query = @"INSERT INTO финансовые_операции 
                    (дата_операции, тип_операции, сумма, категория, описание, id_сотрудника, id_участка) 
                    VALUES (@дата, @тип, @сумма, @категория, @описание, @сотрудник, @участок)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(comboBoxТип.Text) ||
                        string.IsNullOrEmpty(textBoxСумма.Text) ||
                        string.IsNullOrEmpty(textBoxКатегория.Text) ||
                        comboBoxСотрудник.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@дата", dateTimePickerДата.Value);
                        command.Parameters.AddWithValue("@тип", comboBoxТип.Text);
                        command.Parameters.AddWithValue("@сумма", decimal.Parse(textBoxСумма.Text));
                        command.Parameters.AddWithValue("@категория", textBoxКатегория.Text);
                        command.Parameters.AddWithValue("@описание", textBoxОписание.Text ?? "");
                        command.Parameters.AddWithValue("@сотрудник", ((KeyValuePair<int, string>)comboBoxСотрудник.SelectedItem).Key);

                        if (comboBoxУчасток.SelectedItem != null)
                            command.Parameters.AddWithValue("@участок", ((KeyValuePair<int, string>)comboBoxУчасток.SelectedItem).Key);
                        else
                            command.Parameters.AddWithValue("@участок", DBNull.Value);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            comboBoxТип.SelectedIndex = -1;
                            textBoxСумма.Clear();
                            textBoxКатегория.Clear();
                            textBoxОписание.Clear();
                            comboBoxСотрудник.SelectedIndex = -1;
                            comboBoxУчасток.SelectedIndex = -1;
                            LoadFinanceData();
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
                string query = "DELETE FROM финансовые_операции WHERE id_операции = @id_операции";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@id_операции", textBoxID.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                textBoxID.Clear();
                                LoadFinanceData();
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
                            ф.id_операции,
                            ф.дата_операции,
                            ф.тип_операции,
                            ф.сумма,
                            ф.категория,
                            ф.описание,
                            CONCAT(с.фамилия, ' ', с.имя) AS сотрудник,
                            у.название_участка
                        FROM финансовые_операции ф
                        LEFT JOIN сотрудники с ON ф.id_сотрудника = с.id_сотрудника
                        LEFT JOIN участки у ON ф.id_участка = у.id_участка
                        WHERE ф.категория LIKE @SearchText + '%' OR ф.описание LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Операция не найдена!");
                        }

                        dataGridViewFinance.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message);
                    }
                }
            }
            else
            {
                LoadFinanceData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadFinanceData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
