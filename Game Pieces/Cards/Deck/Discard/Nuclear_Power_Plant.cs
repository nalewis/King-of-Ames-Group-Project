using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Nuclear_Power_Plant : Card
    {
        public override int Cost => 6;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Plus 2 victory points
        /// Heal 3 damage
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.VictoryPoints += 2;
            monster.Health += 3;
        }
    }
}