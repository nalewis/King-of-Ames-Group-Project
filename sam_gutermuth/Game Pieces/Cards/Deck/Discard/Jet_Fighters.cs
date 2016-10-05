using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class Jet_Fighters : Card
    {
        public override int Cost => 5;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// Plus 4 victory points
        /// Take 3 damage
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.VictroyPoints += 5;
            monster.Health -= 4;
        }
    }
}