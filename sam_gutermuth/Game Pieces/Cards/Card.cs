using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Observer_Pattern;
using GamePieces.Monsters;

namespace GamePieces.Cards
{
    public abstract class Card : Observer<Monster>
    {
        public static List<Card> GetCards()
        {
            var cards = typeof(Card)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Card)) && !t.IsAbstract)
                .Select(t => (Card) Activator.CreateInstance(t)).ToList();

            var duplicates = new List<Card>();
            foreach (var card in cards)
            {
                if (card.CardsPerDeck <= 1) continue;
                for (var counter = 0; counter < card.CardsPerDeck - 1; counter++)
                    duplicates.Add((Card) Activator.CreateInstance(card.GetType()));
            }
            cards.AddRange(duplicates);
            return new List<Card>(cards.OrderBy(card => Guid.NewGuid()));
        }

        public string Name => GetType().Name.Replace("_", " ");
        public virtual int Cost => 3;
        public virtual CardType CardType => CardType.Keep;
        public virtual int CardsPerDeck => 1;
        public virtual bool OncePerTurn => CardType == CardType.Discard;
        public bool Activated { get; set; }

        public override bool UpdateCondition(Monster monster)
        {
            if (!MonsterShouldUpdate(monster) || Activated) return false;
            Activated = OncePerTurn;
            return true;
        }

        protected abstract bool MonsterShouldUpdate(Monster monster);

        public virtual void UndoEffect(Monster monster)
        {
        }

        public void Reset()
        {
            Activated = false;
        }
    }
}