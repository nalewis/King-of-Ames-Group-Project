using System;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Energy_Hoarder : Card
    {
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.EndOfTurn;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += monster.Energy / 6;
        }
    }
}