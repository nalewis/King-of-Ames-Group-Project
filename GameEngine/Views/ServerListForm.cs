﻿using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameEngine.ServerClasses;
using Networking;

namespace GameEngine.Views
{
    /// <summary>
    /// Form for the user to view all available servers
    /// </summary>
    public partial class ServerListForm : Form
    {
        /// <summary>
        /// Initializing variables
        /// </summary>
        public ServerListForm()
        {
            InitializeComponent();
            join.Enabled = false;
            ListServers();
        }

        /// <summary>
        /// On click, updates the list of available servers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refresh_Click(object sender, EventArgs e)
        {
            serverList.Items.Clear();
            ListServers();
        }

        /// <summary>
        /// Checks if user is closing the application, closes accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ServerListForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            Dispose();
            Client.ClientStop();
            Environment.Exit(0);
        }

        /// <summary>
        /// On click, takes user back to the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMenu_Click(object sender, EventArgs e)
        {
            var main = new MainMenuForm();
            Client.ClientStop();
            main.Show();
            Dispose();
        }

        /// <summary>
        /// Enabled by selecting a server
        /// On click, joins the selected server and takes the user to the player lobby
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void join_Click(object sender, EventArgs e)
        {
            if (!join.Enabled) return;
            //Checks if user is banned
            if (NetworkClasses.GetPlayer(User.PlayerId).Tables[0].Rows[0]["IsBanned"].ToString() == "0")
            {
                if (Client.Connect())
                {
                    try
                    {
                        NetworkClasses.JoinServer(serverList.SelectedItems[0].SubItems[1].Text, User.PlayerId);
                        //Increments games joined
                        NetworkClasses.UpdateUserValue("User_Stats", "Games_Joined", "Games_Joined+1", User.PlayerId);
                        Form lobby = new PlayerLobby();
                        lobby.Show();
                        Dispose();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                }
                else { Console.WriteLine("Couldn't Connect"); }
            }
            else
            {
                MessageBox.Show("Can't connect to game, you have been banned. Please contact administrator to lift ban.", "Join Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the form view with the current list of servers in the database
        /// </summary>
        private void ListServers()
        {
            var ds = NetworkClasses.GetServers();

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                var grabber = NetworkClasses.GetPlayer(int.Parse(row["Host"].ToString()));

                var listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["Local_IP"].ToString());
                listItem.SubItems.Add(NetworkClasses.GetNumPlayers(int.Parse(row["Server_ID"].ToString())) + "/6");
                listItem.SubItems.Add(row["Status"].ToString());

                //Add the row entry to the listview
                serverList.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Gets Host IP when server is selected, and updates the client connection string
        /// Also enables the join game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //selected items[0] is the row, subitems[1] is the ip
                var data = serverList.SelectedItems[0].SubItems[1].Text;
                Client.Conn = data;
                join.Enabled = true;
                join.BackColor = Color.LightGray;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
