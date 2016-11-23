using System;
using Controllers;
using GamePieces.Session;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            LobbyController.AddPlayer(0, "0");
            LobbyController.AddPlayer(1, "1");
            LobbyController.StartGame();
           GameStateController.AcceptAction(GameStateController.StartTurn());
            GameStateController.AcceptAction(GameStateController.Roll());


            Console.WriteLine(Game.Current.State);
            Console.WriteLine(Game.Monsters[0].State);
        }
    }
}


