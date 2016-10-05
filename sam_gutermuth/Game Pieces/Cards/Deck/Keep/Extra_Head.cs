using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class Extra_Head : Card
    {
        public override CardType CardType => CardType.Stats;
        public override int Cost => 7;
        public override int CardsPerDeck => 2;
        private bool canUse = true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            if (!canUse) return false;
            canUse = false;
            return true;
        }

        protected override void UpdateLogic(Monster monster)
        {
            monster.Dice += 1;
        }

        public override void UndoEffect(Monster monster)
        {
            monster.Dice -= 1;
            canUse = true;
        }
    }
}