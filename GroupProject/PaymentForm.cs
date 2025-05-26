using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupProject
{
    public partial class PaymentForm : Form
    {
        int userID;
        string UserName;
        double TotalAmount;
        int TransactionID;
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Desktop\Event Driven Programming\GroupProject\GroupProject\Database1.mdf;Integrated Security=True";

        public PaymentForm(int userID, double totalAmount, int transactionID, string userName)
        {
            InitializeComponent();
            this.userID = userID;
            this.TotalAmount = totalAmount;
            this.TransactionID = transactionID;
            UserName = userName;
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"{UserName}";
            label3.Text = $"RM{TotalAmount}";
            dateTimePicker1.MinDate = DateTime.Now;
            dateTimePicker1.MaxDate = DateTime.Now.AddDays(10);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserID = 0;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();

            this.Hide();
            new LoginPage().Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please choose a method");
            }
            else
            {
                string MethodChosen = comboBox1.SelectedItem.ToString();
                DateTime DatePayout = dateTimePicker1.Value;
                string PayoutStatus = "Pending";

                if (comboBox1.SelectedIndex == 0)
                {
                    string query = @"INSERT INTO Payment (UserID, TransactionID, AmountPaid, Method, PaymentDate, Status)
                                     VALUES (@UserID, @TransactionID, @Amount, @Method, @Date, @Status)";
                    using (SqlConnection conn = new SqlConnection(connection))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                        cmd.Parameters.AddWithValue("@Amount", TotalAmount);
                        cmd.Parameters.AddWithValue("@Method", MethodChosen);
                        cmd.Parameters.AddWithValue("@Date", DatePayout);
                        cmd.Parameters.AddWithValue("@Status", PayoutStatus);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        MessageBox.Show("Payout Successfully, Please wait for admin to confirm");
                    }
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    if (string.IsNullOrEmpty(textBox2.Text) || numericUpDown1.Value == 0  || comboBox2.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please fill all bank details");
                        return;
                    }
                    string AccNum = numericUpDown1.Value.ToString();
                    string AccName = textBox2.Text.Trim();
                    string Bank = comboBox2.SelectedItem.ToString();

                    string query = @"INSERT INTO Payment (UserID, TransactionID, AmountPaid, Method, PaymentDate, Status, [Account Number], [Bank Name], [Account Name])
                                     VALUES (@UserID, @TransactionID, @Amount, @Method, @Date, @Status, @AccNum, @Bank, @AccName)";
                    using (SqlConnection conn = new SqlConnection(connection))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                        cmd.Parameters.AddWithValue("@Amount", TotalAmount);
                        cmd.Parameters.AddWithValue("@Method", MethodChosen);
                        cmd.Parameters.AddWithValue("@Date", DatePayout);
                        cmd.Parameters.AddWithValue("@Status", PayoutStatus);
                        cmd.Parameters.AddWithValue("@AccNum", AccNum);
                        cmd.Parameters.AddWithValue("@Bank", Bank);
                        cmd.Parameters.AddWithValue("@AccName", AccName);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        MessageBox.Show("Payout Successfully, Please wait for admin to confirm");
                        
                    }
                }
                this.Hide();
                new SubmissionForm(userID).Show();
            }
        }
        bool isTransfer;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                isTransfer = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                numericUpDown1.Visible = true; 
                comboBox2.Visible  = true;
                textBox2.Visible = true;
            }
            else
            {
                isTransfer = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                numericUpDown1.Visible = false;
                comboBox2.Visible = false;
                textBox2.Visible = false;
            }
        }
    }
}
