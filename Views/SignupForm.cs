using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Networking;
using Controllers.Helpers;

namespace Views
{
    //TODO add error message checking
    public partial class NewUserForm : Form
    {
        public NewUserForm()
        {
            InitializeComponent();
        }

        private void newUserButton_Click(object sender, EventArgs e)
        {
            if (newUsername.TextLength > 0 && newPassword.TextLength > 0)
            {
                //hide label from previous failures
                errorLabel.Hide();
                //NewUser.handleUserInput(newUsername.Lines[0], newPassword.Lines[0], errorLabel);
                var result = NetworkClasses.createUser(newUsername.Lines[0], newPassword.Lines[0], Helpers.GetLocalIPAddress());
                Console.WriteLine(result);
            }
        }

        private void loginLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form form = new LoginForm();
            form.Show();
            this.Hide();
        }
    }
}
