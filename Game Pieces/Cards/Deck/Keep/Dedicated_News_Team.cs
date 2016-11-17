using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Dedicated_News_Team : Card
    {
        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.NumberOfCards > monster.PreviousNumberOfCards && monster.State == State.BuyingCard;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += 1;
        }
    }
}