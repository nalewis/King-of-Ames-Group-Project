using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Tanks : Card
    {
        public override int Cost => 4;
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 4;
            monster.Health -= 3;
        }
    }
}