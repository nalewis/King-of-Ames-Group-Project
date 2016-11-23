﻿using System;
using System.Windows.Forms;
using GameEngine.ServerClasses;
using Networking;

namespace GameEngine.Views
{
    /// <summary>
    /// Form to list the players currently in the lobby and select character
    /// </summary>
    public partial class PlayerLobby : Form
    {
        //Timer to handle view updates
        private readonly Timer _timer;

        /// <summary>
        /// Initializing variables
        /// </summary>
        public PlayerLobby()
        {
            InitializeComponent();
            UpdateList();

            _timer = new Timer {Interval = (1*1000)};//Ticks every 1 seconds
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Updates the list of players and their characters every 1 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateList();
        }

        /// <summary>
        /// On click, resets character to null, removes player from server, stops NetClient, and takes user back to main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveGame_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            NetworkClasses.UpdateCharacter(User.PlayerId, null);
            NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
            Client.ClientStop();
            Form form = new MainMenuForm();
            form.Show();
            Dispose();
        }

        /// <summary>
        /// Checks if user is closing the application, clsoes accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlayerLobby_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            _timer.Stop();
            Dispose();
            NetworkClasses.UpdateCharacter(User.PlayerId, null);
            NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
            Client.ClientStop();
            Environment.Exit(0);
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void UpdateList()
        {
            playerList.Items.Clear();
            char_list.Items.Clear();
            char_list.Items.Add("Alienoid");
            char_list.Items.Add("Cyber Bunny");
            char_list.Items.Add("Giga Zaur");
            char_list.Items.Add("Kraken");
            char_list.Items.Add("Meka Dragon");
            char_list.Items.Add("The King");
            char_list.Items.Add("The Real King"); //TODO This unlocks something cool, is this enough tho or will this break things?
            try
            {
                var ds = NetworkClasses.GetServer(Client.Conn);
                if (ds.Tables[0].Rows[0]["Status"].ToString() == "In Progress")
                {
                    _timer.Stop();
                    Dispose();
                }
                var row = ds.Tables[0].Rows[0];

                var grabber = NetworkClasses.GetPlayer(int.Parse(row["Host"].ToString()));
                var Character = "";

                //Host
                var listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                Character = grabber.Tables[0].Rows[0]["_Character"].ToString();
                listItem.SubItems.Add(Character);
                if (char_list.Items.Contains(Character))
                {
                    char_list.Items.Remove(Character);
                }

                //Add the row entry to the listview
                playerList.Items.Add(listItem);

                for (var i = 2; i <= 6; i++)
                {
                    if (string.IsNullOrEmpty(row["Player_" + i].ToString())) continue;
                    grabber = NetworkClasses.GetPlayer(int.Parse(row["Player_" + i].ToString()));
                    listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                    Character = grabber.Tables[0].Rows[0]["_Character"].ToString();
                    listItem.SubItems.Add(Character);
                    if (char_list.Items.Contains(Character))
                    {
                        char_list.Items.Remove(Character);
                    }
                    playerList.Items.Add(listItem);
                }
            }
            catch (Exception) //Thrown if server no longer exists
            {
                NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
                Form form = new MainMenuForm();
                form.Show();
                _timer.Stop();
                Client.ClientStop();
                NetworkClasses.UpdateCharacter(User.PlayerId, null);
                MessageBox.Show("Host left the game", "Server Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
            }
        }

        private void char_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                NetworkClasses.UpdateCharacter(User.PlayerId, char_list.SelectedItem.ToString());
                UpdateList();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid character", "Please choose a valid character", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
