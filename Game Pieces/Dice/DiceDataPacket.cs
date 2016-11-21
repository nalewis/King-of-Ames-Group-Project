using System;
using GamePieces.Cards;
using System.Collections.Generic;
using System.Linq;

namespace GamePieces.Dice
{
    [Serializable]
    public struct DiceDataPacket
    {
        public int Size { get; }
        public Symbol[] Symbols { get; }
        public Color[] Colors { get; }
        public bool[] States { get; }

        public DiceDataPacket(List<Die> Dice)
        {
            Size = Dice.Count;
            Symbols = Dice.Select(die => die.Symbol).ToArray();
            Colors = Dice.Select(die => die.Color).ToArray();
            States = Dice.Select(die => die.Save).ToArray();
        }
    }
}