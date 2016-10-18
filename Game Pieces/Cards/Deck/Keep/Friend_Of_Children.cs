using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Friend_Of_Children : Card
    {
        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.Energy > monster.PreviousEnergy && monster.State == State.Energizing;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Energy += 1;
        }
    }
}