using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Apartment_Building : Card
    {
        public override int Cost => 5;
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 3;
        }
    }
}