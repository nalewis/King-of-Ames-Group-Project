﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Controllers;
using GameEngine.ServerClasses;
using Networking;

namespace GameEngine.Views
{
    /// <summary>
    /// Form to list the players currently in the lobby and select character w/ ability to start the game
    /// </summary>
    public partial class HostGameListForm : Form
    {
        //Timer to facilitate the updating of the view
        private readonly Timer _timer;
        private readonly Form _chat = new LobbyChat();

        /// <summary>
        /// Initializing variables
        /// </summary>
        public HostGameListForm()
        {
            InitializeComponent();
            _chat.Show();
            start_game.Enabled = false;
            UpdateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            _timer = new Timer {Interval = (1*1000)}; //Ticks every 1 seconds
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        /// <summary>
        /// Automatic update of the list of players and their characters every 1 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (Host.Players.Count > 1)
            {
                start_game.Enabled = NetworkClasses.CheckReady(Host.Players);
            }
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
                _chat.Dispose();
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
            NetworkClasses.UpdateUserValue("User_List", "_Character", null, User.PlayerId);
            Host.ServerStop();
            Form form = new MainMenuForm();
            form.Show();
            _chat.Dispose();
            Dispose();
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void UpdateList()
        {
            //Resets the view
            playerList.Items.Clear();

            //Gets server info and puts it into a dataset
            var ds = NetworkClasses.GetServer(User.PlayerId, User.LocalIp);
            var row = ds.Tables[0].Rows[0];
            var grabber = NetworkClasses.GetPlayer(int.Parse(row["Host"].ToString()));
            var character = "";

            //Host
            var listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
            character = grabber.Tables[0].Rows[0]["_Character"].ToString();
            listItem.SubItems.Add(character);

            //Add the clients to the listview
            playerList.Items.Add(listItem);
            for(var i = 2; i <= 6; i++)
            {
                if (string.IsNullOrEmpty(row["Player_" + i].ToString())) continue;
                grabber = NetworkClasses.GetPlayer(int.Parse(row["Player_" + i].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                character = grabber.Tables[0].Rows[0]["_Character"].ToString();
                listItem.SubItems.Add(character);

                playerList.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Once all players are ready, adds players to the Game controller and starts the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_game_Click(object sender, EventArgs e)
        {
            var ds = NetworkClasses.GetServer(User.PlayerId, User.LocalIp);
            var row = ds.Tables[0].Rows[0];

            var grabber = NetworkClasses.GetPlayer(int.Parse(row["Host"].ToString()));

            //Host
            LobbyController.AddPlayer(int.Parse(grabber.Tables[0].Rows[0]["Player_ID"].ToString()), grabber.Tables[0].Rows[0]["_Character"].ToString());

            for (var i = 2; i <= 6; i++)
            {
                if (string.IsNullOrEmpty(row["Player_" + i].ToString())) continue;
                grabber = NetworkClasses.GetPlayer(int.Parse(row["Player_" + i].ToString()));
                LobbyController.AddPlayer(int.Parse(grabber.Tables[0].Rows[0]["Player_ID"].ToString()), grabber.Tables[0].Rows[0]["_Character"].ToString());
            }
            NetworkClasses.UpdateServerValue("Status", "In Progress", "Host", User.PlayerId);
            LobbyController.StartGame();
            Host.StartGame();
            _timer.Stop();
            _chat.Dispose();
            Dispose();
        }

        private void char_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (NetworkClasses.CheckCharacterAvailable(Client.Conn, char_list.SelectedItem.ToString()))
                {
                    NetworkClasses.UpdateUserValue("User_List", "_Character", char_list.SelectedItem.ToString(), User.PlayerId);
                }
                else
                {
                    MessageBox.Show("Character Unavailable", "Character has already been selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid character", "Please choose a valid character", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
