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

//TODO add error message checking

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
                NetworkClasses.createUser(newUsername.Lines[0], newPassword.Lines[0], Helpers.GetLocalIPAddress()); 
            }
        }

        /// <summary>
        /// On click, sends user back to sign-in menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form form = new LoginForm();
            form.Show();
            this.Dispose();
        }
    }
}
