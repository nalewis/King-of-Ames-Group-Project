﻿using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Energize : Card
    {
        public override int Cost => 8;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Gain 9 energy
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
           monster.Energy += 9;
        }
    }
}