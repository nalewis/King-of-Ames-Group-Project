using System;
using System.Windows.Forms;
using Networking;
using Controllers.Helpers;

namespace Views
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if(usernameBox.TextLength > 0 && passwordBox.TextLength > 0)
            {
                errorLabel.Hide();
                //TODO encrypt password
                //LoginStuff.handleUserInput(usernameBox.Lines[0], passwordBox.Lines[0], this, errorLabel);
                if(NetworkClasses.login(usernameBox.Lines[0], passwordBox.Lines[0], Helpers.GetLocalIPAddress()))
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

        private void createAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form form = new NewUserForm();
            form.Show();
            this.Hide();
        }
    }
}
