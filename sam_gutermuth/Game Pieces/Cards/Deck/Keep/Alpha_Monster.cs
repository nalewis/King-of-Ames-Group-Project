using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Alpha_Monster : Card
    {
        public override int Cost => 5;
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.Attacking && monster.AttackPoints > 0;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 1;
        }
    }
}