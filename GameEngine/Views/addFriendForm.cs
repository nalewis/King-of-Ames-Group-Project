using System;
using System.Linq;
using System.Windows.Forms;
using Networking;

namespace GameEngine.Views
{
    public partial class AddFriendForm : Form
    {
        public AddFriendForm()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (username.Text.Length > 0)
            {
                if(ContainsVaildChars(username.Text))
                {
                    if (NetworkClasses.AddFriend(username.Text))
                    {
                        username.Text = "";
                        Hide();
                    }
                    else { MessageBox.Show("Username doesn't exist", "Error?", MessageBoxButtons.OK, MessageBoxIcon.Error);}
                }
                else
                {
                    MessageBox.Show("Username can only contian letters and numbers", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Checks if all characters in the given string are valid
        /// Valid chars include 0-9, A-Z, a-z, and spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true if valid, false otherwise</returns>
        private static bool ContainsVaildChars(string s)
        {
            return s.All(t => (t > 47 && t < 58) || (t > 64 && t < 91) || (t > 96 && t < 123) || t == 32);
        }
    }
}
