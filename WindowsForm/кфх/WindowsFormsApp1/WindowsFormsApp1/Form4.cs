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
        private string connection = "Data Source=PC;Initial Catalog=КФХ_Горбов_НИ;Integrated Security=True;Encrypt=False";
        public Form4()
        {
            InitializeComponent();
            LoadPlotsData();
        }
        private void LoadPlotsData()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                        у.id_участка,
                        у.название_участка,
                        CONCAT(с.фамилия, ' ', с.имя) AS ответственный,
                        у.площадь_га,
                        к.название_культуры,
                        у.дата_посева,
                        у.планируемая_дата_сбора
                    FROM участки у
                    LEFT JOIN сотрудники с ON у.ответственный_сотрудник = с.id_сотрудника
                    LEFT JOIN сельхоз_культуры к ON у.id_культуры = к.id_культуры";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridViewPlots.DataSource = table;
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
                string query = @"INSERT INTO участки 
                    (название_участка, ответственный_сотрудник, площадь_га, id_культуры, дата_посева, планируемая_дата_сбора) 
                    VALUES (@название, @ответственный, @площадь, @культура, @посев, @сбор)";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    if (string.IsNullOrEmpty(textBoxНазвание.Text) ||
                        comboBoxОтветственный.SelectedItem == null ||
                        string.IsNullOrEmpty(textBoxПлощадь.Text))
                    {
                        MessageBox.Show("Заполните все обязательные поля!");
                        return;
                    }

                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@название", textBoxНазвание.Text);
                        command.Parameters.AddWithValue("@ответственный", ((KeyValuePair<int, string>)comboBoxОтветственный.SelectedItem).Key);
                        command.Parameters.AddWithValue("@площадь", decimal.Parse(textBoxПлощадь.Text));

                        if (comboBoxКультура.SelectedItem != null)
                            command.Parameters.AddWithValue("@культура", ((KeyValuePair<int, string>)comboBoxКультура.SelectedItem).Key);
                        else
                            command.Parameters.AddWithValue("@культура", DBNull.Value);

                        command.Parameters.AddWithValue("@посев", dateTimePickerПосев.Value);

                        if (dateTimePickerСбор.Checked)
                            command.Parameters.AddWithValue("@сбор", dateTimePickerСбор.Value);
                        else
                            command.Parameters.AddWithValue("@сбор", DBNull.Value);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Данные добавлены!");
                            textBoxНазвание.Clear();
                            comboBoxОтветственный.SelectedIndex = -1;
                            comboBoxКультура.SelectedIndex = -1;
                            textBoxПлощадь.Clear();
                            LoadPlotsData();
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
                string query = "DELETE FROM участки WHERE id_участка = @id_участка";

                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@id_участка", textBoxID.Text);
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Данные удалены!");
                                textBoxID.Clear();
                                LoadPlotsData();
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
                            у.id_участка,
                            у.название_участка,
                            CONCAT(с.фамилия, ' ', с.имя) AS ответственный,
                            у.площадь_га,
                            к.название_культуры,
                            у.дата_посева,
                            у.планируемая_дата_сбора
                        FROM участки у
                        LEFT JOIN сотрудники с ON у.ответственный_сотрудник = с.id_сотрудника
                        LEFT JOIN сельхоз_культуры к ON у.id_культуры = к.id_культуры
                        WHERE у.название_участка LIKE @SearchText + '%'";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        adapter.SelectCommand.Parameters.AddWithValue("@SearchText", searchText);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count == 0)
                        {
                            MessageBox.Show("Участок не найден!");
                        }

                        dataGridViewPlots.DataSource = table;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при поиске: " + ex.Message);
                    }
                }
            }
            else
            {
                LoadPlotsData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadPlotsData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
