using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class High_Altitude_Bombing : Card
    {
        public override int Cost => 4;
        public override CardType CardType => CardType.Discard;

        /// <summary>
        /// All monsters (including you) take 3 damage
        /// </summary>
        /// <param name="monster">Monster</param>
        protected override void UpdateLogic(Monster monster)
        {
            monster.Monsters.ForEach(player => player.Health -= 3);
        }
    }
}