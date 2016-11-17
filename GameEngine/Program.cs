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
            LobbyController.AddPlayer(1,"The King");
            LobbyController.AddPlayer(2, "Kraken");
            LobbyController.AddPlayer(3, "Cyber Bunny");
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}