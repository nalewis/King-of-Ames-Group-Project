using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lidgren.Network;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Specialized;


namespace LoginScreenWinForm
{
    public partial class ServerListForm : Form
    {
        Client client = new Client();
        string server;
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

        private void mainMenu_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            client.clientStop();
            main.Show();
            this.Dispose();
        }

        private void join_Click(object sender, EventArgs e)
        {
            if(join.Enabled)
            {
                bool conn = client.connect();
                if(conn)
                {
                    Console.WriteLine("Connected");
                    //client.joinServer();
                }
                else { Console.WriteLine("Couldn't Connect"); }
            }
        }

        private void formListServers()
        {
            List<string> servers = client.listServers();
            /*for (int i = 0; i < servers.Count; i++)
            {
                serverList.Items.Add(servers.ElementAt(i));
            }*/
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var data = serverList.SelectedItems[0].Text;
            Console.WriteLine("Selected game's IP is: " + data);
            /*if(data != null && data.ToString().Contains("IP"))
            {
                Console.WriteLine(data);
                server = data.ToString();
                server = server.Split(':',' ')[4];
                client.conn = server;
            }*/
            join.Enabled = true;
            join.BackColor = Color.LightGray;
        }
    }
}
