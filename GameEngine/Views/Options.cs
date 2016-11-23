using Networking;
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
                    messageLabel.Text = "Successfully updated username to " + nameChangeText.Lines[0];
                    nameChangeText.Lines[0] = "";
                }
                else
                {
                    messageLabel.Text = "Invalid/Unavailable username";
                }
                messageLabel.Visible = true;
            }
        }

        private void banPlayer_Click(object sender, EventArgs e)
        {
            messageLabel.Visible = false;
            messageLabel.Text = "";

            if (NetworkClasses.IsAdmin(User.PlayerId))
            {
                if (banPlayerText.TextLength > 0)
                {
                    if (NetworkClasses.BanPlayer(User.PlayerId))
                    {
                        messageLabel.Text = "Successfully Banned Player ID: " + banPlayerText.Lines[0];
                        banPlayerText.Lines[0] = "";
                    }
                    else
                    {
                        messageLabel.Text = "Invalid Player ID";
                    }
                }
            }
            else
            {
                messageLabel.Text = "You are not an admin, contact an admin for privileges";
            }
            messageLabel.Visible = true;
        }
    }
}
