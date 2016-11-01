﻿using Controllers.test;
using System;
using System.Drawing;
using System.Windows.Forms;


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
                    Console.WriteLine(serverList.SelectedItems[0].SubItems[0].Text + " : " + serverList.SelectedItems[0].SubItems[1].Text);
                    client.joinServer(serverList.SelectedItems[0].SubItems[0].Text, serverList.SelectedItems[0].SubItems[1].Text);
                }
                else { Console.WriteLine("Couldn't Connect"); }
            }
        }

        private void formListServers()
        {
            ServerDetails[] servers = client.listServers();
            foreach (ServerDetails server in servers)
            {
                ListViewItem listItem = new ListViewItem(server.hostname);
                listItem.SubItems.Add(server.hostip);
                //listItem.SubItems.Add(server.playerDetails.ToString());

                //Add the row entry to the listview
                serverList.Items.Add(listItem);
            }
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //selected items[0] is the row, subitems[1] is the ip
            var data = serverList.SelectedItems[0].SubItems[1].Text;
            Console.WriteLine("Selected game's IP is: " + data);
            client.conn = data;
            join.Enabled = true;
            join.BackColor = Color.LightGray;
        }
    }
}