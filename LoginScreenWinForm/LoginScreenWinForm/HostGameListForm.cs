using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace LoginScreenWinForm
{
    public partial class HostGameListForm : Form
    {
        Host host = new Host();
        public HostGameListForm()
        {
            InitializeComponent();
            playerList.Items.Add(host.hostIP);
            Thread update = new Thread(updateForm);
            update.Start();
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
            host.delServer();
            host.serverStop();
        }

        private static void updateForm()
        {
            int i = 0;
            while(true)
            {
                if(i%1000 == 0)
                {
                    playerList.Items.Add(Host.listUsers);
                }
                i++;
            }
        }
    }
}
