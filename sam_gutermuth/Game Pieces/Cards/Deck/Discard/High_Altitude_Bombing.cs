using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Discard
{
    public class High_Altitude_Bombing : Card
    {
        public override int Cost => 4;
        public override CardType CardType => CardType.Discard;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Monsters.ForEach(player => player.Health -= 3);
        }
    }
}