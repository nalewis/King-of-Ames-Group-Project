using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Heal : Card
    {
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Health += 2;
        }
    }
}