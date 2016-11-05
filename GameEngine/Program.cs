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
            LobbyController.AddPlayer(2, "TurdFerguson");
            LobbyController.AddPlayer(3, "OhHai");
            LobbyController.AddPlayer(3, "LastTest");
            LobbyController.StartGame();
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}
