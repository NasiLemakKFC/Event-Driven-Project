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

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoGenerateColumns = false; 

            SetupDataGridViewColumns();
        }

        private void SetupDataGridViewColumns()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Id",
                HeaderText = "Id",
                Name = "colId",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Brand/Model",
                HeaderText = "Brand/Model",
                Name = "colBrand",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Category",
                HeaderText = "Category",
                Name = "colCategory",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Name = "colType",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Weight",
                HeaderText = "Weight",
                Name = "colWeight",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "RatePerKg",
                HeaderText = "RatePerKg",
                Name = "colRate",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "SubTotal",
                HeaderText = "SubTotal",
                Name = "colSubtotal",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Desktop\Event Driven Programming\GroupProject\GroupProject\Database1.mdf;Integrated Security=True";

        private void button6_Click_1(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserID = 0;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();

            this.Hide();
            new LoginPage().Show();
        }
        string UserName;

        private void SubmissionForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Now;
            dateTimePicker1.MaxDate = DateTime.Now.AddDays(10);

            string query = "SELECT Username FROM [User] WHERE ID = @userID";
            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(query, conn)) 
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                conn.Open();

                object result = cmd.ExecuteScalar();
                conn.Close();
                UserName = result.ToString();

                if (result != null)
                {
                    CustomerNameLabel.Text = $"{result.ToString()} Items";
                    UserHistoryLabel.Text = $"{result.ToString()} History";
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

            //history

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.DataBoundItem != null)
            {
                int itemId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colId"].Value);

                string query = "DELETE FROM Items WHERE Id = @ItemId";

                using (SqlConnection conn = new SqlConnection(connection))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemId", itemId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("Item removed successfully");
                LoadItems();
            }
            else
            {
                MessageBox.Show("Please select an item to remove");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to remove all items?",
                                       "Confirm Clear All",
                                       MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM Items WHERE UserID = @UserID";

                using (SqlConnection conn = new SqlConnection(connection))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                textBox1.Text = "";
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                numericUpDown1.Value = 1;

                MessageBox.Show("All items removed successfully");
                LoadItems();
            }
        }

        private void CalculateTotal()
        {
            if (dataGridView1.Rows.Count != 0)
            {
                if (radioButton1.Checked || radioButton2.Checked)
                {
                    decimal total = 0m;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        total += Convert.ToDecimal(row.Cells["colSubtotal"].Value);
                    }

                    if (radioButton2.Checked)
                    {
                        decimal diskon = 0.03m * total;
                        total -= diskon;
                    }
                    TotalItemLabel.Visible = true;
                    TotalItemLabel.Text = $"Total Price {total:F2}";
                    button5.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Please choose either Pickup or Self-delivered");
                }
            }
            else
            {
                MessageBox.Show("Item is Empty");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CalculateTotal();
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
            CalculateTotal();
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
            string query = "SELECT Id, [Brand/Model], Category, Type, Weight, RatePerKg, Subtotal FROM Items WHERE UserID = @UserID AND TransactionID IS NULL";

            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                UpdateDataGridView(dt);
            }
        }
        private void UpdateDataGridView(DataTable dt)
        {
            dataGridView1.SuspendLayout();

            try
            {
                dataGridView1.DataSource = null;
                dataGridView1.Columns.Clear();
                SetupDataGridViewColumns();

                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            finally
            {
                dataGridView1.ResumeLayout();
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
            if (comboBox1.SelectedIndex != -1)
            {
                string category = comboBox1.SelectedItem.ToString();

                if (category == "Consumer Electronics")
                    comboBox2.Items.AddRange(new string[] { "Mobile Phone", "Laptop", "TV", "Printer", "Monitor" });
                else if (category == "Batteries")
                    comboBox2.Items.AddRange(new string[] { "Mobile Battery", "Laptop Battery", "AA/AAA Battery" });
                else if (category == "Large Appliances")
                    comboBox2.Items.AddRange(new string[] { "Refrigerator", "Washing Machine", "Air Conditioner" });
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }
        double TotalWeight = 0;
        private void button5_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                TotalWeight += Convert.ToDouble(row.Cells["colWeight"].Value);
            }
            double TotalAmount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                TotalAmount += Convert.ToDouble(row.Cells["colSubtotal"].Value);
            }

            string deliveryType = radioButton1.Checked ? "Self-Delivered" : "Collect";

            string insertTransaction = @"INSERT INTO [Transaction] (UserID, SubmissionDate, TotalWeight, TotalAmount, DelieveryType)
                                         VALUES (@UserID, @Date, @Weight, @Amount, @Type);
                                         SELECT SCOPE_IDENTITY();";

            int TransactionID;

            using (SqlConnection conn = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(insertTransaction, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Weight", TotalWeight);
                cmd.Parameters.AddWithValue("@Amount", TotalAmount);
                cmd.Parameters.AddWithValue("@Type", deliveryType);

                conn.Open();
                TransactionID = Convert.ToInt32(cmd.ExecuteScalar());   
                conn.Close();
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int itemId = Convert.ToInt32(row.Cells["colId"].Value);

                string updateItemQuery = "UPDATE Items SET TransactionID = @TransactionID WHERE Id = @ItemID";

                using (SqlConnection conn = new SqlConnection(connection))
                using (SqlCommand updateCmd = new SqlCommand(updateItemQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                    updateCmd.Parameters.AddWithValue("@ItemID", itemId);

                    conn.Open();
                    updateCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            new PaymentForm(userID,TotalAmount, TransactionID, UserName).Show();
            this.Hide();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
