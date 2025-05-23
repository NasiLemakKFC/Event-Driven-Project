using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace GroupProject
{
    public partial class LoginPage : Form
    {   
        public LoginPage()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        { 
        }

        private void AdminLabel_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked)
            {
                isAdmin = true;
                UserLabel.Visible = false;
                UserTextBox.Visible = false;
                AdminLabel.Visible = true;
                AdminTextBox.Visible = true;
            }
            else
            {
                isAdmin = false;
                UserLabel.Visible = true;
                UserTextBox.Visible = true;
                AdminLabel.Visible = false;
                AdminTextBox.Visible = false;
            }
        }

        bool isAdmin = false;

        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Desktop\Event Driven Programming\GroupProject\GroupProject\Database1.mdf;Integrated Security=True";

        private void button1_Click(object sender, EventArgs e)
        {
            if (isAdmin)
            {
                if (string.IsNullOrEmpty(AdminTextBox.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Please Fill all fields");
                    return;
                }
                else
                {
                    string adminUser = AdminTextBox.Text;
                    string adminPass = textBox3.Text;

                    string query = "SELECT ID FROM [Admin] WHERE Name = @adminUser AND Password = @adminPass";
                    
                    using (SqlConnection sqlConnection = new SqlConnection(connection))
                    {
                        SqlCommand cmd = new SqlCommand(query, sqlConnection);
                        cmd.Parameters.AddWithValue("@adminUser", adminUser);
                        cmd.Parameters.AddWithValue("@adminPass", adminPass);
                        
                        sqlConnection.Open();
                        object result = cmd.ExecuteScalar();
                        sqlConnection.Close();

                        if (result != null)
                        {
                            int AdminID = Convert.ToInt32(result);
                            if (checkBox1.Checked)
                            {
                                Properties.Settings.Default.AdminID = AdminID;
                                Properties.Settings.Default.RememberMe = true;
                            }
                            else
                            {
                                Properties.Settings.Default.AdminID = 0;
                                Properties.Settings.Default.RememberMe = false;
                            }
                            Properties.Settings.Default.Save();
                            new AdminForm().Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Admin Name or Password");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(UserTextBox.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Please Fill all fields");
                    return;
                }
                else
                {
                    string userText = UserTextBox.Text;
                    string userPass = textBox3.Text;

                    string query = "SELECT ID FROM [User] WHERE Username = @userText AND Password = @userPass";


                    using (SqlConnection sqlConnection = new SqlConnection(connection))
                    {
                        SqlCommand cmd = new SqlCommand(query, sqlConnection);
                        cmd.Parameters.AddWithValue("@userText", userText);
                        cmd.Parameters.AddWithValue("@userPass", userPass);

                        sqlConnection.Open();
                        object result = cmd.ExecuteScalar();
                        sqlConnection.Close();

                        if (result != null)
                        {
                            int userID = Convert.ToInt32(result);
                            if (checkBox1.Checked)
                            {
                                Properties.Settings.Default.UserID = userID;
                                Properties.Settings.Default.RememberMe = true;
                            }
                            else
                            {
                                Properties.Settings.Default.UserID = 0;
                                Properties.Settings.Default.RememberMe = false;
                            }
                            Properties.Settings.Default.Save();
                            new SubmissionForm(userID).Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password");
                            return;
                        }
                    }
                }
            }
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {
                textBox3.PasswordChar = '\0';
            }
            else
            {
                textBox3.PasswordChar = '*';
            }
        }
    }
}
