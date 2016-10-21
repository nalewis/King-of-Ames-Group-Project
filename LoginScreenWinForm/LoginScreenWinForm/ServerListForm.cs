﻿using System;
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
        public ServerListForm()
        {
            InitializeComponent();
            List<string> servers = client.listServers();
            for(int i = 0;i<servers.Count;i++)
            {
                serverList.Items.Add(servers.ElementAt(i));
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            client.listServers();
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
        }
    }
}
