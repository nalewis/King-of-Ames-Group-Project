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
    public partial class PlayerLobby : Form
    {
        public PlayerLobby()
        {
            InitializeComponent();
        }

        private void leaveGame_Click(object sender, EventArgs e)
        {
            MainMenuForm main = new MainMenuForm();
            main.Show();
            this.Dispose();
        }

        public void PlayerLobby_Closing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Dispose();
                Client.clientStop();
                Environment.Exit(0);
            }
        }

        private void playerList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
