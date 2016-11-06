using System;
using Controllers;

namespace GameEngine
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            LobbyController.AddPlayer(0, "Bill");
            LobbyController.AddPlayer(1, "Ted");
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}
