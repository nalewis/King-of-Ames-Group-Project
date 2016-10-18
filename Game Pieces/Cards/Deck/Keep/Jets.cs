using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Jets : Card
    {
        public override int Cost => 5;
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.Yielding;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Health = monster.PreviousHealth;
        }
    }
}