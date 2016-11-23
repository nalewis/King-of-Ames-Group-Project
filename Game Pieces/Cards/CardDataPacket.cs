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
        public string[] Name { get; set; }
        public int[] Cost { get; set; }
        public CardType[] CardType { get; set; }
        public int[] CardsPerDeck { get; set; }
        public bool[] OncePerTurn { get; set; }
        public bool[] Activated { get; set; }


        public CardDataPacket(List<Card> cards)
        {
            Size = cards.Count;
            Name = cards.Select(card => card.Name).ToArray();
            Cost = cards.Select(card => card.Cost).ToArray();
            CardType = cards.Select(card => card.CardType).ToArray();
            CardsPerDeck = cards.Select(card => card.CardsPerDeck).ToArray();
            OncePerTurn = cards.Select(card => card.OncePerTurn).ToArray();
            Activated = cards.Select(card => card.Activated).ToArray();
        }
    }
}