using System;
using System.Collections.Generic;
using System.Linq;
using GamePieces.Dice;

namespace GamePieces.Session
{
    public class DiceRoller
    {
        private readonly Random Random = new Random();
        private readonly int Faces = Enum.GetNames(typeof(Symbol)).Length;

        private readonly Stack<Die> Black, Green;

        private readonly int[] Tally;

        public List<Die> Rolling = new List<Die>();

        public int Attack => Tally[(int) Symbol.Attack];
        public int Energy => Tally[(int) Symbol.Energy];
        public int Heal => Tally[(int) Symbol.Heal];

        public int VictroyPoints =>
            (Tally[(int) Symbol.One] >= 3 ? 1 + Tally[(int) Symbol.One] - 3 : 0) +
            (Tally[(int) Symbol.Two] >= 3 ? 2 + Tally[(int) Symbol.Two] - 3 : 0) +
            (Tally[(int) Symbol.Three] >= 3 ? 3 + Tally[(int) Symbol.Three] - 3 : 0);

        public DiceRoller()
        {
            Black = new Stack<Die>(Enumerable.Range(0, 6).Select(die => new Die(Color.Black, Random, Faces)));
            Green = new Stack<Die>(Enumerable.Range(0, 2).Select(die => new Die(Color.Green, Random, Faces)));
            Tally = new int[Faces];
        }

        public void Setup(int dice)
        {
            Array.Clear(Tally, 0, Tally.Length);
            if (Rolling.Count == dice) return;
            while (Rolling.Count != 0)
            {
                if (Rolling[0].Color == Color.Black) Black.Push(Rolling[0]);
                else Green.Push(Rolling[0]);
                Rolling.RemoveAt(0);
            }
            while (dice-- > 0 && Black.Count != 0) Rolling.Add(Black.Pop());
            while (dice-- > 0 && Green.Count != 0) Rolling.Add(Green.Pop());
        }

        public void Roll()
        {
            Array.Clear(Tally, 0, Tally.Length);
            foreach (var die in Rolling)
            {
                die.Roll();
                Tally[(int) die.Symbol]++;
            }
        }

        public void EndRolling()
        {
            Array.Clear(Tally, 0, Tally.Length);
        }
    }
}