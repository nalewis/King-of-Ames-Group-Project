using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controllers.User;
using Networking;

namespace Views
{
    /// <summary>
    /// Form to handle user navigation to the various pre-game menus, options, and account info
    /// </summary>
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the application if the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainMenuForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Dispose();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// On click, starts the NetHost and takes user to the host lobby 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostButton_Click(object sender, EventArgs e)
        {
            Host.serverStart();
            NetworkClasses.updatePlayerStat(User.id, "Games_Hosted", 1);
            Form gameList = new HostGameListForm();
            gameList.Show();
            this.Dispose();
        }

        /// <summary>
        /// On click, starts the NetClient and takes user to the server list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoinButton_Click(object sender, EventArgs e)
        {
            Client._client.Start();
            Form serverList = new ServerListForm();
            serverList.Show();
            this.Dispose();
        }
    }
}
