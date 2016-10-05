using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Fire_Blast : Card
    {
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Deal 2 damage to all other monsters
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.Monsters.Where(enemy => !enemy.Equals(monster))
                .ToList()
                .ForEach(enemy => enemy.Health -= 2);
        }
    }
}