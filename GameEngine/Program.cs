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
            LobbyController.AddPlayer(0, "One");
            LobbyController.AddPlayer(1, "Two");
            LobbyController.AddPlayer(2, "Three");
            LobbyController.AddPlayer(3, "Four");
            LobbyController.AddPlayer(4, "Five");
            LobbyController.AddPlayer(5, "Six");
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}