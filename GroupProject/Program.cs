using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupProject
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Properties.Settings.Default.RememberMe)
            {
                if (Properties.Settings.Default.AdminID != 0)
                {
                    Application.Run(new AdminForm());
                }
                else
                {
                    Application.Run(new SubmissionForm(Properties.Settings.Default.UserID));
                }
            }
            else
            {
                Application.Run(new LoginPage());
            }
        }
    }
}
