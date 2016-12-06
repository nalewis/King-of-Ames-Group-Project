using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly Form _chat = new LobbyChat();
        private readonly List<DataSet> _players = new List<DataSet>();

        /// <summary>
        /// Initializing variables
        /// </summary>
        public PlayerLobby()
        {
            InitializeComponent();
            _chat.Show();
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
            NetworkClasses.UpdateUserValue("User_List", "_Character", null, User.PlayerId);
            NetworkClasses.UpdateUserValue("User_List", "Online", "Online", User.PlayerId);
            NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
            Client.ClientStop();
            Form form = new MainMenuForm();
            form.Show();
            _chat.Dispose();
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
            _chat.Dispose();
            Dispose();
            NetworkClasses.UpdateUserValue("User_List", "_Character", null, User.PlayerId);
            NetworkClasses.UpdateUserValue("User_List", "Online", "Offline", User.PlayerId);
            NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
            Client.ClientStop();
            Environment.Exit(0);
        }

        private void PlayerLobby_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 'c') return;
            if (_chat.Visible)
            {
                _chat.Hide();
            }
            else
            {
                _chat.Show();
            }
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void UpdateList()
        {
            var done = true;
            try
            {
                //Gets server info and places it into a dataset
                var ds = NetworkClasses.GetServer(Client.Conn);

                //Checks if game has started
                if (ds.Tables[0].Rows[0]["Status"].ToString() == "In Progress")
                {
                    NetworkClasses.UpdateUserValue("User_List", "Online", "In Game", User.PlayerId);
                    MainMenuForm form = new MainMenuForm();
                    form.Show();
                    _chat.Dispose();
                    _timer.Stop();
                    Dispose();
                }

                //Checks if number of players has changed
                var currentNumPlayers = NetworkClasses.GetNumPlayers(int.Parse(ds.Tables[0].Rows[0]["Server_ID"].ToString()));
                if (_players.Count == currentNumPlayers && _players.Count >= 1)
                {
                    //Check if characters have changed
                    var newPlayerChars = new List<DataSet>();
                    newPlayerChars.Add(NetworkClasses.GetPlayer(int.Parse(ds.Tables[0].Rows[0]["Host"].ToString())));
                    for (var i = 2; i <= 6; i++)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Player_" + i].ToString()))
                        {
                            newPlayerChars.Add(NetworkClasses.GetPlayer(int.Parse(ds.Tables[0].Rows[0]["Player_" + i].ToString())));
                        }
                    }
                    for (var i = 0; i < newPlayerChars.Count; i++)
                    {
                        if (_players[i].Tables[0].Rows[0]["_Character"].ToString() !=
                            newPlayerChars[i].Tables[0].Rows[0]["_Character"].ToString())
                        {
                            done = false;
                            break;
                        }
                    }
                    if (done)
                    {
                        return;
                    }
                }
                //Updating
                _players.Clear();
                playerList.Items.Clear();
                var row = ds.Tables[0].Rows[0];

                var grabber = NetworkClasses.GetPlayer(int.Parse(row["Host"].ToString()));
                var Character = "";

                //Host
                _players.Add(grabber);
                var listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                Character = grabber.Tables[0].Rows[0]["_Character"].ToString();
                listItem.SubItems.Add(Character);

                //Add the row entry to the listview
                playerList.Items.Add(listItem);

                for (var i = 2; i <= 6; i++)
                {
                    if (!string.IsNullOrEmpty(row["Player_" + i].ToString()))
                    {
                        grabber = NetworkClasses.GetPlayer(int.Parse(row["Player_" + i].ToString()));
                        _players.Add(grabber);
                        listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                        Character = grabber.Tables[0].Rows[0]["_Character"].ToString();
                        listItem.SubItems.Add(Character);
                        playerList.Items.Add(listItem);
                    }
                }
            }
            catch (Exception) //Thrown if server no longer exists
            {
                //If the host leaves, the server no longer exists and the removing the player will throw an exception
                try
                {
                    NetworkClasses.FindRemovePlayer(Client.Conn, User.PlayerId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                _chat.Dispose();
                Form form = new MainMenuForm();
                form.Show();
                _timer.Stop();
                Client.ClientStop();

                NetworkClasses.UpdateUserValue("User_List", "_Character", null, User.PlayerId);
                NetworkClasses.UpdateUserValue("User_List", "Online", "Online", User.PlayerId);
                Dispose();
                MessageBox.Show("Host left the game", "Server Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void char_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(NetworkClasses.CheckCharacterAvailable(Client.Conn, char_list.SelectedItem.ToString()))
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
