using System;
using System.Drawing;
using System.Windows.Forms;
using Networking;
using System.Data;

namespace Views
{
    public partial class ServerListForm : Form
    {
        public ServerListForm()
        {
            InitializeComponent();
            join.Enabled = false;
            formListServers();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            serverList.Items.Clear();
            formListServers();
        }

        public void ServerListForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Dispose();
                Client.clientStop();
                Environment.Exit(0);
            }
        }

        private void mainMenu_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            Client.clientStop();
            main.Show();
            this.Dispose();
        }

        private void join_Click(object sender, EventArgs e)
        {
            if(join.Enabled)
            {
                bool conn = Client.connect();
                if(conn)
                {
                    Console.WriteLine("Connected");
                    Console.WriteLine(serverList.SelectedItems[0].SubItems[0].Text + " : " + serverList.SelectedItems[0].SubItems[1].Text);
                    //client.joinServer(serverList.SelectedItems[0].SubItems[0].Text, serverList.SelectedItems[0].SubItems[1].Text);
                }
                else { Console.WriteLine("Couldn't Connect"); }
            }
        }

        private void formListServers()
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

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //selected items[0] is the row, subitems[1] is the ip
            var data = serverList.SelectedItems[0].SubItems[1].Text;
            Console.WriteLine("Selected game's IP is: " + data);
            Client.conn = data;
            join.Enabled = true;
            join.BackColor = Color.LightGray;
        }
    }
}
