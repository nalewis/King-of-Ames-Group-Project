﻿using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Apartment_Building : Card
    {
        public override int Cost => 5;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Plus 3 victory points
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 3;
        }
    }
}