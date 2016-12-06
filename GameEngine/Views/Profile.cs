using System;
using System.Windows.Forms;
using Networking;

namespace GameEngine.Views
{
    public partial class Profile : Form
    {
        public Profile()
        {
            InitializeComponent();
            InitializeStats();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Form menu = new MainMenuForm();
            menu.Show();
            Dispose();
        }

        private void InitializeStats()
        {
            joinedGames.Text = NetworkClasses.GetUserStat("Games_Joined");
            hostedGames.Text = NetworkClasses.GetUserStat("Games_Hosted");
            wonGames.Text = NetworkClasses.GetUserStat("Games_Won");
        }

        /// <summary>
        /// Checks if user is closing the application, closes accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Profile_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            Dispose();
            Environment.Exit(0);
        }
    }
}
