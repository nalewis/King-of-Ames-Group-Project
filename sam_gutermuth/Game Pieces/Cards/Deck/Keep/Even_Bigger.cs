using System;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Even_Bigger : Card
    {
        public override CardType CardType => CardType.Stats;
        public override int Cost => 4;
        private bool canUse = true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            if (!canUse) return false;
            canUse = false;
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.MaximumHealth += 2;
            monster.Health += 2;
        }

        public override void UndoEffect(Monster monster)
        {
            monster.MaximumHealth -= 2;
            if (monster.Health > monster.MaximumHealth)
                monster.Health = monster.MaximumHealth;
        }
    }
}