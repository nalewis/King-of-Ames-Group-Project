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
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void HostButton_Click(object sender, EventArgs e)
        {
            Form gameList = new HostGameListForm();
            gameList.Show();
            this.Dispose();
        }

        private void JoinButton_Click(object sender, EventArgs e)
        {
            Form serverList = new ServerListForm();
            serverList.Show();
            this.Dispose();
        }
    }
}
