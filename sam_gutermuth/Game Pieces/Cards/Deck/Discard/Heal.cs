using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Heal : Card
    {
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Heal 2 damage
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.Health += 2;
        }
    }
}