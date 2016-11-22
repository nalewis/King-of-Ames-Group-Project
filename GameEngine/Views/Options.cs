﻿using Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine.Views
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Form menu = new MainMenuForm();
            menu.Show();
            Dispose();
        }

        private void nameChange_Click(object sender, EventArgs e)
        {
            messageLabel.Visible = false;
            messageLabel.Text = "";
            if (nameChangeText.TextLength > 0)
            {
                if(NetworkClasses.UpdateUsername(User.PlayerId, nameChangeText.Lines[0]))
                {
                    messageLabel.Visible = true;
                    messageLabel.Text = "Successfully updated username";
                }
                else
                {
                    messageLabel.Visible = true;
                    messageLabel.Text = "Invalid/Unavailable username";
                }
            }
        }
    }
}
