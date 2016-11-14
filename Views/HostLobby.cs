using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Controllers.User;
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
        Timer timer;
        public HostGameListForm()
        {
            InitializeComponent();
            start_game.Enabled = false;
            updateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            timer = new Timer();
            timer.Interval = (2 * 1000); // 2 secs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

        }

        /// <summary>
        /// Automatic update of the list of players and their characters every 2 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            start_game.Enabled = NetworkClasses.checkReady(Host.players);
            updateList();
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
                timer.Stop();
                this.Dispose();
                Host.serverStop();
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
            timer.Stop();
            NetworkClasses.updateCharacter(User.id, null);
            Host.serverStop();
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void updateList()
        {
            playerList.Items.Clear();
            DataSet ds = NetworkClasses.getServer(User.id, User.localIp);
            DataRow row = ds.Tables[0].Rows[0];

            DataSet grabber = NetworkClasses.getPlayer(Int32.Parse(row["Host"].ToString()));
            List<int> pings = new List<int>();
            while(pings.Count < 1)
            {
                pings = Host.getPing();
            }

            //Host
            ListViewItem listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
            listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
            listItem.SubItems.Add(pings[0].ToString() + " ms");
            //listItem.SubItems.Add(NetworkClasses.getNumPlayers(Int32.Parse(row["Server_ID"].ToString())) + "/6");
            //listItem.SubItems.Add(row["Status"].ToString());

            //Add the row entry to the listview
            playerList.Items.Add(listItem);

            if (!String.IsNullOrEmpty(row["Player_2"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_2"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                listItem.SubItems.Add(pings[1].ToString() + " ms");
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_3"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_3"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                listItem.SubItems.Add(pings[2].ToString() + " ms");
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_4"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_4"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                listItem.SubItems.Add(pings[3].ToString() + " ms");
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_5"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_5"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                listItem.SubItems.Add(pings[4].ToString() + " ms");
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_6"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_6"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                listItem.SubItems.Add(pings[5].ToString() + " ms");
                playerList.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Sends the selected character to the database update function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void select_char_Click(object sender, EventArgs e)
        {
            NetworkClasses.updateCharacter(User.id, char_list.SelectedItem.ToString());
        }

        /// <summary>
        /// Once all players are ready, adds players to the Game controller and starts the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_game_Click(object sender, EventArgs e)
        {
            //TODO
            int[] players = NetworkClasses.getPlayerIDs(User.localIp);
            for(var i = 0; i < players.Length; i++)
            {
                if(players[i] != -1)
                {
                    DataSet ds = NetworkClasses.getPlayer(players[i]);
                    LobbyController.AddPlayer(players[i], ds.Tables[0].Rows[0]["_Character"].ToString());
                }
            }
            LobbyController.StartGame();
        }
    }
}
