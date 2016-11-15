using System;
using System.Drawing;
using System.Windows.Forms;
using Networking;
using System.Data;

namespace Views
{
    public partial class ServerListForm : Form
    {
        /// <summary>
        /// Form for the user to view all available servers
        /// </summary>
        public ServerListForm()
        {
            InitializeComponent();
            join.Enabled = false;
            listServers();
        }

        /// <summary>
        /// On click, updates the list of available servers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refresh_Click(object sender, EventArgs e)
        {
            serverList.Items.Clear();
            listServers();
        }

        /// <summary>
        /// When the window is closed, the NetClient is stopped and closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ServerListForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Dispose();
                Client.clientStop();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// On click, takes user back to the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMenu_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            Client.clientStop();
            main.Show();
            this.Dispose();
        }

        /// <summary>
        /// Enabled by selecting a server
        /// On click, joins the selected server and takes the user to the player lobby
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void join_Click(object sender, EventArgs e)
        {
            if(join.Enabled)
            {
                bool goodConnection = Client.connect();
                if(goodConnection)
                {
                    NetworkClasses.joinServer(serverList.SelectedItems[0].SubItems[1].Text, User.id);
                    NetworkClasses.updatePlayerStat(User.id, "Games_Joined", 1);
                    PlayerLobby lobby = new PlayerLobby();
                    lobby.Show();
                    this.Dispose();
                }
                else { Console.WriteLine("Couldn't Connect"); }
            }
        }

        /// <summary>
        /// Updates the form view with the current list of servers in the database
        /// </summary>
        private void listServers()
        {
            DataSet ds = NetworkClasses.getServers();

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                DataSet grabber = NetworkClasses.getPlayer(Int32.Parse(row["Host"].ToString()));

                ListViewItem listItem = new ListViewItem(grabber.Tables[0].Rows[0]["Username"].ToString());
                listItem.SubItems.Add(grabber.Tables[0].Rows[0]["Local_IP"].ToString());
                listItem.SubItems.Add(NetworkClasses.getNumPlayers(Int32.Parse(row["Server_ID"].ToString())) + "/6");
                listItem.SubItems.Add(row["Status"].ToString());

                //Add the row entry to the listview
                serverList.Items.Add(listItem);
            }
        }

        /// <summary>
        /// Gets Host IP when server is selected, and updates the client connection string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //selected items[0] is the row, subitems[1] is the ip
            var data = serverList.SelectedItems[0].SubItems[1].Text;
            Client.conn = data;
            join.Enabled = true;
            join.BackColor = Color.LightGray;
        }
    }
}
