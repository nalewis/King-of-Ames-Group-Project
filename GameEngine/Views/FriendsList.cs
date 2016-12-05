using System;
using System.Windows.Forms;
using Networking;

namespace GameEngine.Views
{
    public partial class FriendsList : Form
    {
        private readonly Timer _timer;
        private string[] _old;
        private readonly Form _add;

        public FriendsList()
        {
            InitializeComponent();
            delFriend.Enabled = false;
            _add = new AddFriendForm();
            GetFriends();
            _timer = new Timer {Interval = 2000};
            _timer.Tick += timer_tick;
            _timer.Start();
        }

        public void timer_tick(object sender, EventArgs e)
        {
            GetFriends();
        }

        public void GetFriends()
        {
            var ds = NetworkClasses.GetPlayer(User.PlayerId);
            if (ds.Tables[0].Rows[0]["Friends"].ToString() == "0")
            {
                BoxOFriends.Items.Clear();
                return;
            }
            var friends = ds.Tables[0].Rows[0]["Friends"].ToString().Split(',');
            if(friends.Length == 20) { addFriend.Enabled = false; }
            else if (friends.Length < 20 && !addFriend.Enabled)
            {
                addFriend.Enabled = true;
            }
            if (_old != null && friends.Length == _old.Length)
            {
                    return;
            }
            BoxOFriends.Items.Clear();
            _old = friends;
            try
            {
                foreach (var friend in friends)
                {
                    ds = NetworkClasses.GetPlayer(int.Parse(friend));
                    var item = new ListViewItem(ds.Tables[0].Rows[0]["Username"].ToString());
                    item.SubItems.Add(ds.Tables[0].Rows[0]["Online"].ToString() == "1" ? "Online" : "Offline");
                    BoxOFriends.Items.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void addFriend_Click(object sender, EventArgs e)
        {
            _add.Show();
        }

        private void FriendsList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                _timer.Stop();
                return;
            }
            _add.Hide();
            e.Cancel = true;
            Hide();
        }

        private void FriendsList_Disposed(object sender, EventArgs e)
        {
            _add.Dispose();
        }

        private void BoxOFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            delFriend.Enabled = true;
        }

        private void delFriend_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure?","Confirm",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                NetworkClasses.DelFriend(BoxOFriends.SelectedItems[0].Text);
            }
        }

        private void FriendsList_KeyPressed(object sender, KeyPressEventArgs e)
        {
            _add.Hide();
            Hide();
        }
    }
}
