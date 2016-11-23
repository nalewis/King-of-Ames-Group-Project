using System;
using System.Windows.Forms;
using GameEngine.ServerClasses;
using Networking;

namespace GameEngine.Views
{
    public partial class LobbyChat : Form
    {
        public LobbyChat()
        {
            InitializeComponent();
            //Chat.LoadFile();
        }

        private void sendMessage_Click(object sender, EventArgs e)
        {
            Client.SendChatMessage(writeMessage.Text + "\n");
            Chat.Text += writeMessage.Text + "\n";
            writeMessage.Text = User.Username + ": ";
        }

        private void clearChat_Click(object sender, EventArgs e)
        {
            Chat.Text = "";
        }
    }
}
