using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Evacuation_Orders : Card
    {
        public override int Cost => 7;
        public override CardType CardType => CardType.Discard;
        public override int CardsPerDeck => 2;

        /// <summary>
        /// All other monsters lose 5 victory points
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.Monsters.Where(enemy => !enemy.Equals(monster))
                .ToList()
                .ForEach(enemy => enemy.VictroyPoints -= 5);
        }
    }
}