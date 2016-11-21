using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameEngine.Views
{
    public partial class Profile : Form
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Form menu = new MainMenuForm();
            menu.Show();
            Dispose();
        }
    }
}
