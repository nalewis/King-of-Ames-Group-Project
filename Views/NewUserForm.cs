using System;
using System.Windows.Forms;
using Networking;

namespace Views
{
    /// <summary>
    /// Form to handle creation of a new user
    /// </summary>
    public partial class NewUserForm : Form
    {
        public NewUserForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On click, sends username,password, and IP to createUser function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUserButton_Click(object sender, EventArgs e)
        {
            //Check that the inputs are not empty
            if (newUsername.TextLength > 0 && newPassword.TextLength > 0)
            {
                //hide label from previous failures
                errorLabel.Hide();
                bool good = NetworkClasses.CreateUser(newUsername.Lines[0], newPassword.Lines[0], Helpers.GetLocalIpAddress());
                if (good)
                {
                    Form form = new LoginForm();
                    form.Show();
                    Dispose();
                }
                else
                {
                    errorLabel.Text = "Username already exists.";
                    errorLabel.Show();
                }
            }
            else
            {
                errorLabel.Text = "Inputs cannot be empty.";
                errorLabel.Show();
            }
        }

        /// <summary>
        /// On click, sends user back to sign-in menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toLogin_Click(object sender, EventArgs e)
        {
            Form form = new LoginForm();
            form.Show();
            Dispose();
        }

        private void NewUserForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Dispose();
                Environment.Exit(0);
            }
        }
    }
}
