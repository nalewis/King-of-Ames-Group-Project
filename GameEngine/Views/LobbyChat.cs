using System;
using System.IO;
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
            Chat.LoadFile("ChatFile.txt", RichTextBoxStreamType.PlainText);
            username.Text = User.Username + ": ";
        }

        private void sendMessage_Click(object sender, EventArgs e)
        {
            //Client.SendChatMessage(writeMessage.Text + "\n");
            StreamWriter write = new StreamWriter("ChatFile.txt", true);
            write.Write(writeMessage.Text);
            writeMessage.Text = "";
        }

        private void clearChat_Click(object sender, EventArgs e)
        {
            Chat.Text = "";
        }
    }
}
