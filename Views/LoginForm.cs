using System;
using System.Windows.Forms;
using Networking;
using Controllers.Helpers;

//TODO encrypt password

namespace Views
{
    /// <summary>
    /// Form to handle exsiting user login
    /// </summary>
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On click, queries the database to check if user exsits, and input is correct
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            //Check input isn't empty, else error message is shown
            if(usernameBox.TextLength > 0 && passwordBox.TextLength > 0)
            {
                //Hide existing error label, if any
                errorLabel.Hide();

                //Sends input to login function, if input is good, sends user to main menu 
                //Else error message is shown
                if (NetworkClasses.login(usernameBox.Lines[0], passwordBox.Lines[0], Helpers.GetLocalIPAddress()))
                {
                    Form form = new MainMenuForm();
                    form.Show();
                    this.Hide();
                }
                else
                {
                    errorLabel.Show();
                }
            }
            else
            {
                errorLabel.Show();
            }
        }

        /// <summary>
        /// On click, takes user to user creation form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form form = new NewUserForm();
            form.Show();
            this.Hide();
        }
    }
}
