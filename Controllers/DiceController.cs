using System.Collections.Generic;
using GamePieces.Dice;
using GamePieces.Session;
using System;

namespace Controllers
{
    public static class DiceController
    {
        /// <summary>
        /// Gets all of the dice being rolled
        /// </summary>
        /// <returns>Dice</returns>
        public static List<Die> GetDice()
        {
            return DiceRoller.Rolling;
        }

        /// <summary>
        /// Save the die at the given index
        /// </summary>
        /// <param name="index">Index</param>
        public static void SaveDie(int index)
        {
            if(index < 0 || index > DiceRoller.Rolling.Count) return;
            DiceRoller.Rolling[index].Save = true;
        }

        /// <summary>
        /// Un-save the die at the given index
        /// </summary>
        /// <param name="index">Index</param>
        public static void UnSaveDie(int index)
        {
            if(index < 0 || index > DiceRoller.Rolling.Count) return;
            DiceRoller.Rolling[index].Save = false;
        }

        /// <summary>
        /// Roll the dice
        /// </summary>
        public static void Roll()
        {
            Console.WriteLine(Game.Current.Equals(Game.Monsters[0]));
            Game.Roll();
        }

        /// <summary>
        /// End rolling dice
        /// </summary>
        public static void EndRolling()
        {
            Game.EndRolling();
        }
    }
}