using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Skyscraper : Card
    {
        public override int Cost => 6;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Plus 4 victory points
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += 4;
        }
    }
}