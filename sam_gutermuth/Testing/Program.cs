using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GamePieces.Monsters;
using GamePieces.Session;
using ZUnit;

namespace Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // UnitTests.Run<Card_Tests>(true);
            //  UnitTests.Run<Board_Tests>(true);

            var consoleGame = new ConsoleGame();
            consoleGame.Play();
        }
    }
}


