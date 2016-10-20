using System;

namespace GameEngine
{
#if WINDOWS || LINUX
    public static class EngineTest
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}
