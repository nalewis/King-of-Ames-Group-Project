﻿using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Vast_Storm : Card
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
            monster.Monsters.Where(enemy => !enemy.Equals(monster))
                .ToList()
                .ForEach(enemy => enemy.Energy -= enemy.Energy / 2);
        }
    }
}