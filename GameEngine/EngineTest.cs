using System;
using System.Collections.Generic;

namespace GameEngine
{
#if WINDOWS || LINUX
    public static class EngineTest
    {
        [STAThread]
        static void Main()
        {
            Controllers.Test.StartGame(new List<string>() { "Bob", "Bill" });
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}
