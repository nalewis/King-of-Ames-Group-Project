using GamePieces.Monsters;
using GamePieces.Session;
using ZUnit;

namespace Testing.Game_Pieces_Tests
{
    public class Board_Tests
    {
        private static readonly GameComponents GameComponents = new GameComponents();

        private static Monster MakeMonster(int attackPoints = 0)
        {
            return new Monster(GameComponents) {AttackPoints = attackPoints};
        }

        private static Monster[] MakeMonsters(int count)
        {
            var monsters = new Monster[count];
            for (var i = 0; i < count; i++)
                monsters[i] = MakeMonster();
            return monsters;
        }

        public static string Attack_And_Move_To_Tokyo()
        {
            var monster = MakeMonster(1);
            monster.Attack();
            GameComponents.Reset();
            return UnitTests.Compare(Location.TokyoCity, monster.Location);
        }

        public static string Attack_And_Enemy_Does_Not_Yield()
        {
            var monster = MakeMonster(1);
            var enemy = MakeMonster();
            GameComponents.Board.MoveIntoTokyo(enemy);
            monster.Attack();
            GameComponents.Reset();
            return UnitTests.Compare(new dynamic[] {Location.Default, monster.Location, Location.TokyoCity, enemy.Location});
        }

        public static string Attack_And_Enemy_Yields()
        {
            var monster = MakeMonster(1);
            var enemy = MakeMonster();
            GameComponents.Board.MoveIntoTokyo(enemy);
            monster.Attack();
            enemy.Yield();
            GameComponents.Reset();
            return UnitTests.Compare(new dynamic[] {Location.TokyoCity, monster.Location, Location.Default, enemy.Location});
        }

        public static string Attack_And_Move_To_Tokyo_Bay()
        {
            var enemies = MakeMonsters(5);
            GameComponents.Board.MoveIntoTokyo(enemies[0]);
            var monster = MakeMonster(1);
            monster.Attack();
            GameComponents.Reset();
            return
                UnitTests.Compare(new dynamic[]
                    {Location.TokyoBay, monster.Location, Location.TokyoCity, enemies[0].Location});
        }
    }
}