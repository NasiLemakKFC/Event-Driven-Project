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

namespace GroupProject
{
    public partial class SubmissionForm : Form
    {
        int userID;
        public SubmissionForm(int userID)
        {
            InitializeComponent();
            this.userID = userID;
        }

        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Desktop\Event Driven Programming\GroupProject\GroupProject\Database1.mdf;Integrated Security=True";


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TotalItemLabel_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserID = 0;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();

            this.Hide();
            new LoginPage().Show();
        }

        private void SubmissionForm_Load(object sender, EventArgs e)
        {
            string query = "SELECT Username FROM [User] WHERE ID = @userID";
            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(query, conn)) 
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                conn.Open();

                object result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null)
                {
                    CustomerNameLabel.Text = $"{result.ToString()} Items";
                }
                else
                {
                    CustomerNameLabel.Text = "Unknown User";
                }
            }

            comboBox1.Items.AddRange(new string[] {
                "Consumer Electronics",
                "Batteries",
                "Large Appliances"
            });
            LoadItems();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            numericUpDown1.Value = 0;

            dataGridView1.Rows.Clear();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            decimal total = 0m;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }

            if (radioButton2.Checked)
            {
                decimal diskon = 0.02m * total;
                total -= diskon;
            }
            TotalItemLabel.Text = $"Total Price {total:F2}";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                label5.Visible = true;
            }
            else
            {
                label5.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 || numericUpDown1.Value <= 0)
            {
                MessageBox.Show("Please complete all fields.");
                return;
            }

            string brand = textBox1.Text;
            string Category = comboBox1.SelectedItem.ToString();
            string Type = comboBox2.SelectedItem.ToString();
            decimal Weight = numericUpDown1.Value;
            decimal rate = GetRatePerKg(Type);
            decimal Subtotal = rate * Weight;

            string query = @"INSERT INTO Items ([Brand/Model], Category, Type, Weight, RatePerKg, SubTotal, UserID) 
                 VALUES (@Brand, @Category, @Type, @Weight, @RatePerKg, @SubTotal, @UserID)";


            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@Weight", Weight);
                cmd.Parameters.AddWithValue("@RatePerKg", rate);
                cmd.Parameters.AddWithValue("@SubTotal", Subtotal);
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            MessageBox.Show("Item added successfully");
            LoadItems();

        }

        private void LoadItems()
        {
            string query = "SELECT Id, [Brand/Model], Category, Type, Weight, RatePerKg, Subtotal FROM Items WHERE UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@UserID", userID); // only show this user's items
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    dataGridView1.DataSource = null; 
                    dataGridView1.Columns.Clear(); 
                }
            }
        }

        private decimal GetRatePerKg(string type)
        {
            Dictionary<string, decimal> ratePerKg = new Dictionary<string, decimal>()
                {
                    { "Mobile Phone", 100m },
                    { "Laptop", 150m },
                    { "TV", 80m },
                    { "Printer", 60m },
                    { "Monitor", 70m },

                    { "Mobile Battery", 50m },
                    { "Laptop Battery", 60m },
                    { "AA/AAA Battery", 40m },

                    { "Refrigerator", 30m },
                    { "Washing Machine", 35m },
                    { "Air Conditioner", 40m }
                };
            return ratePerKg.ContainsKey(type) ? ratePerKg[type] : 0m;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox2.Items.Clear();
            string category = comboBox1.SelectedItem.ToString();

            if (category == "Consumer Electronics")
                comboBox2.Items.AddRange(new string[] { "Mobile Phone", "Laptop", "TV", "Printer", "Monitor" });
            else if (category == "Batteries")
                comboBox2.Items.AddRange(new string[] { "Mobile Battery", "Laptop Battery", "AA/AAA Battery" });
            else if (category == "Large Appliances")
                comboBox2.Items.AddRange(new string[] { "Refrigerator", "Washing Machine", "Air Conditioner" });
        }
    }
}
