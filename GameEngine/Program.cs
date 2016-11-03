using System;

namespace GameEngine
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new Engine())
                game.Run();
        }
    }
#endif
}
