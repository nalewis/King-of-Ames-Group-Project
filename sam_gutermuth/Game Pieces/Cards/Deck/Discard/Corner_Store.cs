using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Corner_Store : Card
    {
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 1;
        }
    }
}