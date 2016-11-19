using System;
using System.Windows.Forms;
using GameEngine.Views;

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

//        [STAThread]
        public static void Run()
        {
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}