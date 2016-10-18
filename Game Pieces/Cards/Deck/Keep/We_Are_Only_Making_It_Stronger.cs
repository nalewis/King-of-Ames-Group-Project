using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class We_Are_Only_Making_It_Stronger : Card
    {
        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.PreviousHealth - monster.Health >= 2 && monster.State == State.Attacked;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Energy += 1;
        }
    }
}