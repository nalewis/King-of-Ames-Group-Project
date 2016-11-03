using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Controllers.User;
using Networking;
using System.Data;

namespace Views
{
    public partial class HostGameListForm : Form
    {
        Host host = new Host();
        Timer timer;
        public HostGameListForm()
        {
            InitializeComponent();
            updateList();
            //timer that runs to check for updated SQL values, then updates listview accordingly
            timer = new Timer();
            timer.Interval = (5 * 1000); // 5 secs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            updateList();
        }

        public void HostGameListForm_Closing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
                timer.Stop();
                this.Dispose();
                Client.clientStop();
                host.serverStop();
                Environment.Exit(0);
            //}
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            timer.Stop();
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
            NetworkClasses.updateCharacter(User.id, null);
            host.serverStop();
        }

        private void updateList()
        {
            playerList.Items.Clear();

            DataSet ds = NetworkClasses.getServer(User.id, User.localIp);
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

        private void select_char_Click(object sender, EventArgs e)
        {
            NetworkClasses.updateCharacter(User.id, char_list.SelectedItem.ToString());
        }
    }
}
