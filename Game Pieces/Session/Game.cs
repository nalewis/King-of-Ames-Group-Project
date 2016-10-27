using System;
using System.Collections.Generic;
using System.Linq;
using GamePieces.Cards;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public static class Game
    {
        //Monsters
        public static Monster Current { get; set; }

        public static readonly List<Monster>
            Monsters = new List<Monster>(),
            Dead = new List<Monster>(),
            Attacked = new List<Monster>();

        public static int Players => Monsters.Count;

        public static Monster Winner =>
            Players == 1
                ? Monsters.First()
                : Monsters.Exists(monster => monster.VictroyPoints >= 20)
                    ? Monsters.Where(monster => monster.VictroyPoints >= 20).ToList().First()
                    : null;


        //Cards
        public static Stack<Card> Deck;
        public static readonly List<Card> CardsForSale = new List<Card>();

        public static void StartGame(List<int> playerIds, List<string> names)
        {
            if (playerIds.Count != names.Count) throw new Exception("Player IDs and Names must have the same count");
            Deck = new Stack<Card>(Card.GetCards());
            for (var i = 0; i < 3; i++) if (Deck.Count != 0) CardsForSale.Add(Deck.Pop());
            Monsters.Clear();
            Dead.Clear();
            for(var i = 0; i < playerIds.Count; i++) Monsters.Add(new Monster(playerIds[i], names[i]));
            Current = Monsters.First();
            Board.Reset();
        }

        public static void StartTurn()
        {
            Current.StartTurn();
        }

        public static void Roll()
        {
            Current.Roll();
        }

        public static void EndRolling()
        {
            Current.EndRolling();
            Current.Attack();
        }

        public static void BuyCard(int index)
        {
            if (index < 0 || index >= CardsForSale.Count) return;
            var card = CardsForSale[index];
            CardsForSale.RemoveAt(index);
            Current.BuyCard(card);
            if (Deck.Count != 0) CardsForSale.Add(Deck.Pop());
        }

        public static void SellCard(Monster monster, Card card)
        {
            Current.SellCard(monster, card);
        }

        public static void RemoveCard(Card card)
        {
            Current.RemoveCard(card);
        }

        public static void EndTurn()
        {
            Current.EndTurn();
            Current = Current.Next;
        }
    }
}