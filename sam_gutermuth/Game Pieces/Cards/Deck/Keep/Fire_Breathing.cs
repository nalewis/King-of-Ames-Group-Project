using System;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Fire_Breathing : Card
    {
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.Attacking && monster.AttackPoints > 0;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Previous.Health -= 1;
            if (monster.Previous.Equals(monster.Next)) return;
            monster.Next.Health -= 1;
        }
    }
}