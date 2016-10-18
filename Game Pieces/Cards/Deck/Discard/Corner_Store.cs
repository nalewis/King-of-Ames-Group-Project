﻿using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Corner_Store : Card
    {
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Plus 1 victory point
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 1;
        }
    }
}