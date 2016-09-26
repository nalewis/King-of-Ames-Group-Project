using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Fire_Blast : Card
    {
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Monsters.Where(enemy => !enemy.Equals(monster))
                .ToList()
                .ForEach(enemy => enemy.Health -= 2);
        }
    }
}