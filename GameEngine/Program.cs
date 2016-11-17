using System;
using Controllers;
using System.Windows.Forms;
using Views;
using Lidgren.Network;
using GamePieces.Monsters;
using System.Threading;
using Networking;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameEngine
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = new LoginForm();
            form.Show();
            Application.Run();
        }

        [STAThread]
        public static void Run()
        {
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}