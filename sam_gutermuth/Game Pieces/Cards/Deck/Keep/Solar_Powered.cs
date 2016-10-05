using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Solar_Powered : Card
    {
        public override int Cost => 2;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.State == State.EndOfTurn && monster.Energy == 0;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Energy += 1;
        }
    }
}