using System.Collections.Generic;
using GamePieces.Cards;
using GamePieces.Dice;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class Game
    {
        private readonly GameComponents _gameComponents = new GameComponents();

        public Monster Current { get; set; }
        public List<Die> Dice => _gameComponents.DiceRoller.Rolling;
        public DiceRoller DiceRoller => _gameComponents.DiceRoller;
        public Stack<Card> Deck => _gameComponents.Deck;
        public List<Card> CardsForSale => _gameComponents.CardsForSale;
        public List<Monster> Monsters => _gameComponents.Monsters;
        public Board Board => _gameComponents.Board;
        public bool Winner => Monsters.Count == 1 || Monsters.Exists(monster => monster.VictroyPoints >= 20);

        public Game(List<string> names)
        {
            names.ForEach(name => _gameComponents.AddMonster(name));
            Current = _gameComponents.Monsters[0];
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

        public void BuyCard(int index)
        {
            if(index < 0 || index > CardsForSale.Count) return;
            var card = CardsForSale[index];
            CardsForSale.RemoveAt(index);
            Current.BuyCard(card);
            CardsForSale.Add(Deck.Pop());
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