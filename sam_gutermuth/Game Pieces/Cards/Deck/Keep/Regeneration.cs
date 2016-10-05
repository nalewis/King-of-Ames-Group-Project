using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Regeneration : Card
    {
        public override int Cost => 4;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.Health > monster.PreviousHealth && monster.State == State.Healing;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Health += 1;
        }
    }
}