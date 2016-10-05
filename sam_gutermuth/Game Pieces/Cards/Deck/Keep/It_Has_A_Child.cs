using GamePieces.Monsters;

namespace GamePieces.Cards.Deck.Keep
{
    public class It_Has_A_Child : Card
    {
        public override int Cost => 7;
        public override bool OncePerTurn => true;

        protected override bool MonsterShouldUpdate(Monster monster)
        {
            return monster.Health == 0;
        }

        protected override void UpdateLogic(Monster monster)
        {
            while (monster.Cards.Count != 0)
                monster.RemoveCard(monster.Cards[0]);
            monster.VictroyPoints = 0;
            monster.Health = 10;
        }
    }
}