using System.Linq;
using GamePieces.Monsters;
using GamePieces.Session;

namespace GamePieces.Cards.Deck.Keep
{
    public class Rooting_For_The_Underdog : Card
    {
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.EndOfTurn &&
                   Game.Monsters.Where(enemy => !enemy.Equals(monster))
                       .ToList()
                       .All(enemy => monster.VictoryPoints < enemy.VictoryPoints);
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += 1;
        }
    }
}