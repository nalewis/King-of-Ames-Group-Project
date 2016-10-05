
using System;
using System.Collections.Generic;
using GamePieces.Cards;
using GamePieces.Dice;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class Game
    {
        private readonly GameComponents GameComponents = new GameComponents();

        public Monster Current { get; set; }
        public List<Die> Dice => GameComponents.DiceRoller.Rolling;
        public DiceRoller DiceRoller => GameComponents.DiceRoller;
        public Stack<Card> Deck => GameComponents.Deck;
        public List<Card> CardsForSale => GameComponents.CardsForSale;
        public List<Monster> Monsters => GameComponents.Monsters;
        public Board Board => GameComponents.Board;

        public bool Winner => Monsters.Count == 1 || Monsters.Exists(monster => monster.VictroyPoints >= 20);

        public Game(List<string> names)
        {
            names.ForEach(name => GameComponents.AddMonster(name));
            Current = GameComponents.Monsters[0];
        }

        public void StartTurn()
        {
            Current.StartTurn();
        }

        public void Roll()
        {
            Current.Roll();
        }

        public void EndRolling()
        {
            Current.EndRolling();
            Current.Attack();
        }

        public void BuyCard(Card card)
        {
            Current.BuyCard(card);
        }

        public void SellCard(Monster monster, Card card)
        {
            Current.SellCard(monster, card);
        }

        public void RemoveCard(Card card)
        {
            Current.RemoveCard(card);
        }

        public void EndTurn()
        {
            Current.EndTurn();
            Current = Current.Next;
        }
    }
}