using System;
using System.Windows.Forms;

namespace LoginScreenWinForm
{
    //TODO add error message checking
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // Login Button Clicked
            // username = usernameBox.Lines[0]
            // password = passwordBox.Lines[0]
            if(usernameBox.TextLength > 0 && passwordBox.TextLength > 0)
            {
                errorLabel.Hide();
                LoginStuff.handleUserInput(usernameBox.Lines[0], passwordBox.Lines[0], this, errorLabel);
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
