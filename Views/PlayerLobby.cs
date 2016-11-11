using Controllers.User;
using Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Views
{
    /// <summary>
    /// Form to list the players currently in the lobby and select character
    /// </summary>
    public partial class PlayerLobby : Form
    {
        Timer timer;
        public PlayerLobby()
        {
            InitializeComponent();
            updateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            timer = new Timer();
            timer.Interval = (2 * 1000); // 2 secs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Updates the list of players and their characters every 2 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            updateList();
        }

        /// <summary>
        /// On click, resets character to null, removes player from server, stops NetClient, and takes user back to main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveGame_Click(object sender, EventArgs e)
        {
            timer.Stop();
            NetworkClasses.updateCharacter(User.id, null);
            NetworkClasses.findRemovePlayer(Client.conn, User.id);
            Client.clientStop();
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
        }

        public void PlayerLobby_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                timer.Stop();
                this.Dispose();
                NetworkClasses.updateCharacter(User.id, null);
                NetworkClasses.findRemovePlayer(Client.conn, User.id);
                Client.clientStop();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Updates the list of players with the current information about the server via the database
        /// </summary>
        private void updateList()
        {
            playerList.Items.Clear();
            DataSet ds = NetworkClasses.getServer(Client.conn);
            DataRow row = ds.Tables[0].Rows[0];

            DataSet grabber = NetworkClasses.getPlayer(Int32.Parse(row["Host"].ToString()));

            //Host
            ListViewItem listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
            listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
            //listItem.SubItems.Add(NetworkClasses.getNumPlayers(Int32.Parse(row["Server_ID"].ToString())) + "/6");
            //listItem.SubItems.Add(row["Status"].ToString());

            //Add the row entry to the listview
            playerList.Items.Add(listItem);

            if (!String.IsNullOrEmpty(row["Player_2"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_2"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_3"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_3"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_4"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_4"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_5"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_5"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
                playerList.Items.Add(listItem);
            }

            if (!String.IsNullOrEmpty(row["Player_6"].ToString()))
            {
                grabber = NetworkClasses.getPlayer(Int32.Parse(row["Player_6"].ToString()));
                listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["_Character"].ToString());
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
    }
}
