using System.Windows.Forms;
using Networking;

namespace GameEngine.Views
{
    public partial class FriendsList : Form
    {
        public FriendsList()
        {
            InitializeComponent();
            GetFriends();
        }

        public void GetFriends()
        {
            var ds = NetworkClasses.GetPlayer(User.PlayerId);
            var friends = ds.Tables[0].Rows[0]["Friends"].ToString().Split(',');
            foreach (var friend in friends)
            {
                ds = NetworkClasses.GetPlayer(int.Parse(friend));
                var item = new ListViewItem(ds.Tables[0].Rows[0]["Username"].ToString());
                if (ds.Tables[0].Rows[0]["Online"].ToString() == "1") item.SubItems.Add("Online");
                else item.SubItems.Add("Online");
                boxOfriends.Items.Add(item);
            }
        }

        private void mainMenu_Click(object sender, System.EventArgs e)
        {
            Form form = new MainMenuForm();
            form.Show();
            Dispose();
        }
    }
}
