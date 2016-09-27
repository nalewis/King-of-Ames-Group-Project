using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class National_Guard : Card
    {
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 2;
            monster.Health -= 2;
        }
    }
}