using System;
using GamePieces.Monsters;
using GamePieces.Session;
using Testing.Game_Pieces_Tests;
using ZUnit;

namespace Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //UnitTests.Run<Card_Tests>();
            //UnitTests.Run<Board_Tests>();

            var consoleGame = new ConsoleGame();
            consoleGame.Play();
        }
    }
}


