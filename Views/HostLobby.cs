using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Networking;
using System.Data;
using Controllers;

namespace Views
{
    /// <summary>
    /// Form to list the players currently in the lobby and select character w/ ability to start the game
    /// </summary>
    public partial class HostGameListForm : Form
    {
        readonly Timer _timer;
        public HostGameListForm()
        {
            InitializeComponent();
            start_game.Enabled = false;
            UpdateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            _timer = new Timer {Interval = (2*1000)};
            // 2 secs
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        /// <summary>
        /// Automatic update of the list of players and their characters every 2 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            start_game.Enabled = NetworkClasses.CheckReady(Host.Players);
            UpdateList();
        }

        /// <summary>
        /// When the window is closed, the server is stopped, and the application is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HostGameListForm_Closing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
                _timer.Stop();
                Dispose();
                Host.ServerStop();
                Environment.Exit(0);
            //}
        }

        /// <summary>
        /// On click, resets character to null, stops NetServer, and takes user back to main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveGame_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            NetworkClasses.UpdateCharacter(User.Id, null);
            Host.ServerStop();
            MainMenuForm main = new MainMenuForm();
            main.Show();
            Dispose();
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void UpdateList()
        {
            playerList.Items.Clear();
            DataSet ds = NetworkClasses.GetServer(User.Id, User.LocalIp);
            DataRow row = ds.Tables[0].Rows[0];

            DataSet grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Host"].ToString()));
            List<int> pings = new List<int>();
            while(pings.Count < 1)
            {
                pings = Host.GetPing();
            }

            //Host
            ListViewItem listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
            listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
            listItem.SubItems.Add(pings[0].ToString() + " ms");

            //Add the row entry to the listview
            playerList.Items.Add(listItem);
            for(int i = 2; i <= 6; i++)
            {
                if (!String.IsNullOrEmpty(row["Player_" + i].ToString()))
                {
                    grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Player_" + i].ToString()));
                    listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                    listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                    listItem.SubItems.Add(pings[i-1].ToString() + " ms");
                    playerList.Items.Add(listItem);
                }
            }
        }

        /// <summary>
        /// Sends the selected character to the database update function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void select_char_Click(object sender, EventArgs e)
        {
            NetworkClasses.UpdateCharacter(User.Id, char_list.SelectedItem.ToString());
        }

        /// <summary>
        /// Once all players are ready, adds players to the Game controller and starts the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_game_Click(object sender, EventArgs e)
        {
            DataSet ds = NetworkClasses.GetServer(User.Id, User.LocalIp);
            DataRow row = ds.Tables[0].Rows[0];

            DataSet grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Host"].ToString()));

            //Host
            LobbyController.AddPlayer(Int32.Parse(grabber.Tables[0].Rows[0]["Player_ID"].ToString()), grabber.Tables[0].Rows[0]["_Character"].ToString());

            for (int i = 2; i <= 6; i++)
            {
                if (!String.IsNullOrEmpty(row["Player_" + i].ToString()))
                {
                    grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Player_" + i].ToString()));
                    LobbyController.AddPlayer(Int32.Parse(grabber.Tables[0].Rows[0]["Player_ID"].ToString()), grabber.Tables[0].Rows[0]["_Character"].ToString());
                }
            }
            LobbyController.StartGame();
            Host.StartGame();
        }
    }
}
