using System;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Death_From_Above : Card
    {
        public override CardType CardType => CardType.Discard;
        public override int Cost => 5;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 2;
            if (monster.Location == Location.TokyoCity) return;
            monster.Board.LeaveTokyo(monster.Board.TokyoCity);
            monster.Board.MoveIntoTokyo(monster);
        }
    }
}