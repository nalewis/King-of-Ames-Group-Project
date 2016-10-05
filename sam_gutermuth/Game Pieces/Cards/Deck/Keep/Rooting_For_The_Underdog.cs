﻿using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Rooting_For_The_Underdog : Card
    {
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.EndOfTurn &&
                   monster.Monsters.Where(enemy => !enemy.Equals(monster))
                       .ToList()
                       .All(enemy => monster.VictroyPoints < enemy.VictroyPoints);
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 1;
        }
    }
}