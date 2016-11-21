using System;
using System.Windows.Forms;
using GameEngine.ServerClasses;
using Networking;

namespace GameEngine.Views
{
    /// <summary>
    /// Form to handle user navigation to the various pre-game menus, options, and account info
    /// </summary>
    public partial class MainMenuForm : Form
    {
        /// <summary>
        /// Intializing variables
        /// </summary>
        public MainMenuForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Checks if user is closing the application, closes accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainMenuForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            Dispose();
            Environment.Exit(0);
        }

        /// <summary>
        /// On click, starts the NetHost and takes user to the host lobby 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostButton_Click(object sender, EventArgs e)
        {
            Host.ServerStart();
            NetworkClasses.UpdatePlayerStat(User.PlayerId, "Games_Hosted", 1);
            Form gameList = new HostGameListForm();
            gameList.Show();
            Dispose();
        }

        /// <summary>
        /// On click, starts the NetClient and takes user to the server list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoinButton_Click(object sender, EventArgs e)
        {
            Client.NetClient.Start();
            Form serverList = new ServerListForm();
            serverList.Show();
            Dispose();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            Form option = new Options();
            option.Show();
            Dispose();
        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            Form profile = new Profile();
            profile.Show();
            Dispose();
        }
    }
}
