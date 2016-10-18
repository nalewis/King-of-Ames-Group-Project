using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Acid_Attack : Card
    {
        public override int Cost => 6;
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.Attacking;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.AttackPoints += 1;
        }

    }
}