using System.Collections.Generic;
using GamePieces.Dice;
using GamePieces.Session;
using GamePieces.Monsters;

namespace Controllers
{
    public static class Test
    {
        public static void StartGame(List<string> names)
        {
            Game.StartGame(names);
        }

        public static void StartTurn()
        {
            Game.StartTurn();
        }

        public static List<Die> GetDice()
        {
            return DiceRoller.Rolling;
        }

        public static void Roll()
        {
            DiceRoller.Roll();
        }

        public static List<Monster> getMonsters()
        {
            return Game.Monsters;
        }
    }
}