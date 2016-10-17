using System;
using System.Collections.Generic;
using System.Linq;
using GamePieces.Dice;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class DiceRoller
    {
        private readonly Random _random = new Random(); //Random Number Generator
        private readonly int _faces = Enum.GetNames(typeof(Symbol)).Length; //Number of faces for each die

        private readonly Stack<Die> _black, _green; //Black and green dice

        private readonly int[] _tally; //The score of the currently rolled dice

        public List<Die> Rolling = new List<Die>(); //All of thet dice being rollec

        public int Attack => _tally[(int) Symbol.Attack]; //Attack points total
        public int Energy => _tally[(int) Symbol.Energy]; //Energy points total
        public int Heal => _tally[(int) Symbol.Heal]; //Heal points total

        public int VictroyPoints => //Victroy points total
            (_tally[(int) Symbol.One] >= 3 ? 1 + _tally[(int) Symbol.One] - 3 : 0) +
            (_tally[(int) Symbol.Two] >= 3 ? 2 + _tally[(int) Symbol.Two] - 3 : 0) +
            (_tally[(int) Symbol.Three] >= 3 ? 3 + _tally[(int) Symbol.Three] - 3 : 0);

        /// <summary>
        /// A collection of dice that rolls and tallys their values
        /// </summary>
        public DiceRoller()
        {
            _black = new Stack<Die>(Enumerable.Range(0, 6).Select(die => new Die(Color.Black, _random, _faces)));
            _green = new Stack<Die>(Enumerable.Range(0, 2).Select(die => new Die(Color.Green, _random, _faces)));
            _tally = new int[_faces];
        }

        /// <summary>
        /// Setup the dice for the turn
        /// </summary>
        /// <param name="dice">Number of dice to roll</param>
        public void Setup(int dice)
        {
            Array.Clear(_tally, 0, _tally.Length);
            Rolling.ForEach(die => die.Save = false);
            if (Rolling.Count == dice) return;
            while (Rolling.Count != 0)
            {
                if (Rolling[0].Color == Color.Black) _black.Push(Rolling[0]);
                else _green.Push(Rolling[0]);
                Rolling.RemoveAt(0);
            }
            while (dice-- >= 0 && _black.Count != 0) Rolling.Add(_black.Pop());
            while (dice-- >= 0 && _green.Count != 0) Rolling.Add(_green.Pop());
        }

        /// <summary>
        /// Roll all of the dice
        /// </summary>
        public void Roll()
        {
            Array.Clear(_tally, 0, _tally.Length);
            foreach (var die in Rolling)
            {
                die.Roll();
                _tally[(int) die.Symbol]++;
            }
        }

        /// <summary>
        /// Stop rolling for this turn and transfer the tally to the rolling monster
        /// </summary>
        /// <param name="monster">Rolling monster</param>
        public void EndRolling(Monster monster)
        {
            monster.AttackPoints += Attack;
            monster.Energy += Energy;
            monster.Health += Heal;
            monster.VictroyPoints += VictroyPoints;
            Array.Clear(_tally, 0, _tally.Length);
        }
    }
}