using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Urbavore : Card
    {
        public override int Cost => 4;
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.StartOfTurn && monster.InTokyo;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += 1;
            monster.AttackPoints += 1;
        }
    }
}