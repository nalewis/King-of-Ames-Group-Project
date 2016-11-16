using Networking;
using System;
using System.Data;
using System.Windows.Forms;

namespace Views
{
    /// <summary>
    /// Form to list the players currently in the lobby and select character
    /// </summary>
    public partial class PlayerLobby : Form
    {
        readonly Timer _timer;
        public PlayerLobby()
        {
            InitializeComponent();
            UpdateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            _timer = new Timer {Interval = (2*1000)};
            // 2 secs
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Updates the list of players and their characters every 2 seconds
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
            NetworkClasses.UpdateCharacter(User.Id, null);
            NetworkClasses.FindRemovePlayer(Client.Conn, User.Id);
            Client.ClientStop();
            MainMenuForm main = new MainMenuForm();
            main.Show();
            Dispose();
        }

        public void PlayerLobby_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _timer.Stop();
                Dispose();
                NetworkClasses.UpdateCharacter(User.Id, null);
                NetworkClasses.FindRemovePlayer(Client.Conn, User.Id);
                Client.ClientStop();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void UpdateList()
        {
            playerList.Items.Clear();
            try
            {
                DataSet ds = NetworkClasses.GetServer(Client.Conn);
                DataRow row = ds.Tables[0].Rows[0];

                DataSet grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Host"].ToString()));

                //Host
                ListViewItem listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                //Add the row entry to the listview
                playerList.Items.Add(listItem);

                for (int i = 2; i <= 6; i++)
                {
                    if (!String.IsNullOrEmpty(row["Player_" + i].ToString()))
                    {
                        grabber = NetworkClasses.GetPlayer(Int32.Parse(row["Player_" + i].ToString()));
                        listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                        listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                        //listItem.SubItems.Add(pings[i - 1].ToString() + " ms");
                        playerList.Items.Add(listItem);
                    }
                }
            }
            catch (Exception)
            {
                Form form = new MainMenuForm();
                form.Show();
                _timer.Stop();
                Client.ClientStop();
                NetworkClasses.UpdateCharacter(User.Id, null);
                MessageBox.Show("Host left the game", "Server Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Dispose();
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
    }
}
