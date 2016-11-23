using System;
using GamePieces.Cards;
using System.Collections.Generic;
using System.Linq;

namespace GamePieces.Cards
{
    [Serializable]
    public struct CardDataPacket
    {
        public int Size { get; set; }
        public Card[] Cards { get; set; }

        public CardDataPacket(List<Card> cards)
        {
            Size = cards.Count;
            Cards = cards.ToArray();
        }
    }
}