using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Energize : Card
    {
        public override int Cost => 8;
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
           monster.Energy += 9;
        }
    }
}