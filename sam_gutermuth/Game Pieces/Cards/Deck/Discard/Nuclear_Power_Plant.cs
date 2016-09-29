﻿using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Nuclear_Power_Plant : Card
    {
        public override int Cost => 6;
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 2;
            monster.Health += 3;
        }
    }
}