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

            //First row entry is host
            ListViewItem hostItem = new ListViewItem(host.hostName);
            hostItem.SubItems.Add(host.hostIP);

            //Add the row entry to the listview
            playerList.Items.Add(hostItem);

            //timer that runs to check for updated SQL values, then updates listview accordingly
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = (10 * 1000); // 10 secs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            //playerList.Items.Clear();


            foreach (ListViewItem item in playerList.Items)
            {
                //TODO one of these prints out the text data for all the entries in the listview
                //Console.WriteLine(item.Text);
                //Console.WriteLine(item.SubItems);
                //Console.WriteLine(item.SubItems[0]);
                //Console.WriteLine(item.SubItems[0].Text);
            }
                //refresh here...
                //playerList.Items.Add("Hi");
            //Console.WriteLine(playerList.Items.ToString());
        }

        private List<string> updatePlayers()
        {
            return null;
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
            host.delServer();
            host.serverStop();
        }
    }
}
