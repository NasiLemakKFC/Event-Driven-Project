using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                UserLabel.Visible = false;
                UserTextBox.Visible = false;
                AdminLabel.Visible = true;
                AdminTextBox.Visible = true;
            }
            else
            {
                UserLabel.Visible = true;
                UserTextBox.Visible = true;
                AdminLabel.Visible = false;
                AdminTextBox.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
