using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginScreenWinForm
{
    public partial class HostGameListForm : Form
    {
        public HostGameListForm()
        {
            InitializeComponent();
            Host host = new Host();
            playerList.Items.Add(host.hostIP);
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
        }
    }
}
