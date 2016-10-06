using System.Collections.Generic;
using GamePieces.Cards;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class GameComponents
    {
        public readonly List<Monster>
            Monsters = new List<Monster>(),
            Dead = new List<Monster>();

        public readonly Stack<Card> Deck = new Stack<Card>(Card.GetCards());
        public readonly List<Card> CardsForSale = new List<Card>();

        public readonly DiceRoller DiceRoller = new DiceRoller();
        public readonly Board Board;
        public readonly Combat Combat;

        public GameComponents()
        {
            Board = new Board(this);
            Combat = new Combat(this);
            for (var i = 0; i < 3; i++)
                CardsForSale.Add(Deck.Pop());
        }

        public void AddMonster(string name) => new Monster(this, name);

        public void Reset()
        {
            Monsters.Clear();
            Board.Reset();
            Combat.Reset();
        }
    }
}