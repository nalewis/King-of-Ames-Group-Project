using System;
using GamePieces.Cards;
using System.Collections.Generic;

namespace GamePieces.Dice
{
    [Serializable]
    public struct DiceDataPacket
    {
        public Die[] Dice { get; set; }

        public DiceDataPacket(Die[] dice)
        {
            Dice = dice;
        }
    }
}