using GamePieces.Cards;
using GamePieces.Cards.Deck.Discard;
using GamePieces.Cards.Deck.Keep;
using GamePieces.Monsters;
using GamePieces.Session;
using ZUnit;

namespace Testing.Game_Pieces_Tests
{
    public class Card_Tests
    {
        private static readonly GameComponents GameComponents = new GameComponents();

        private static Monster MakeMonster(Card card, int health = 10)
        {
            var monster = new Monster(GameComponents) {Health = health};
            monster.Energy += card.Cost;
            monster.BuyCard(card);
            return monster;
        }

        public static string Apartment_Building()
        {
            var monster = MakeMonster(new Apartment_Building());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(3, monster.VictroyPoints);
        }

        public static string Commuter_Train()
        {
            var monster = MakeMonster(new Commuter_Train());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(2, monster.VictroyPoints);
        }

        public static string Corner_Store()
        {
            var monster = MakeMonster(new Corner_Store());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(1, monster.VictroyPoints);
        }

        public static string Energize()
        {
            var monster = MakeMonster(new Energize());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(9, monster.Energy);
        }

        public static string Evacuation_Orders()
        {
            var enemy = new Monster(GameComponents) {VictroyPoints = 10};
            MakeMonster(new Evacuation_Orders());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(5, enemy.VictroyPoints);
        }

        public static string Fire_Blast()
        {
            var enemy = new Monster(GameComponents);
            var monster = MakeMonster(new Fire_Blast());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {8, enemy.Health, 10, monster.Health});
        }

        public static string Gas_Refinery()
        {
            var enemy = new Monster(GameComponents);
            var monster = MakeMonster(new Gas_Refinery());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {2, monster.VictroyPoints, 7, enemy.Health, 10, monster.Health});
        }

        public static string Heal()
        {
            var monster = MakeMonster(new Heal(), 8);
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(10, monster.Health);
        }

        public static string High_Altitude_Bombing()
        {
            var enemy = new Monster(GameComponents);
            var monster = MakeMonster(new High_Altitude_Bombing());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {7, monster.Health, 7, enemy.Health});
        }

        public static string Jet_Fighters()
        {
            var monster = MakeMonster(new Jet_Fighters());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {5, monster.VictroyPoints, 6, monster.Health});
        }

        public static string National_Guard()
        {
            var monster = MakeMonster(new National_Guard());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {2, monster.VictroyPoints, 8, monster.Health});
        }

        public static string Nuclear_Power_Plant()
        {
            var monster = MakeMonster(new Nuclear_Power_Plant(), 7);
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {2, monster.VictroyPoints, 10, monster.Health});
        }

        public static string Skyscraper()
        {
            var monster = MakeMonster(new Skyscraper());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(4, monster.VictroyPoints);
        }

        public static string Tanks()
        {
            var monster = MakeMonster(new Tanks());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {4, monster.VictroyPoints, 7, monster.Health});
        }

        public static string Vast_Storm()
        {
            var enemyOdd = new Monster(GameComponents) {Energy = 11};
            var enemyEven = new Monster(GameComponents) {Energy = 10};
            var monster = MakeMonster(new Vast_Storm());
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {2, monster.VictroyPoints, 6, enemyOdd.Energy, 5, enemyEven.Energy});
        }

        public static string Acid_Attack()
        {
            var enemy = new Monster(GameComponents) {Location = Location.TokyoCity};
            var monster = MakeMonster(new Acid_Attack());
            monster.Attack();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {10, monster.Health, 9, enemy.Health});
        }

        public static string Alpha_Monster()
        {
            var monster = MakeMonster(new Alpha_Monster());
            monster.Attack();
            monster.AttackPoints += 1;
            monster.Attack();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(1, monster.VictroyPoints);
        }

        public static string Armor_Plating()
        {
            var enemy = new Monster(GameComponents) {Location = Location.TokyoCity, AttackPoints = 5};
            var monster = MakeMonster(new Armor_Plating());
            enemy.Attack();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(6, monster.Health);
        }

        public static string Dedicated_News_Team()
        {
            var monster = MakeMonster(new Dedicated_News_Team());
            var card = new Alpha_Monster();
            monster.Energy += card.Cost;
            monster.BuyCard(card);
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(1, monster.VictroyPoints);
        }

        public static string Energy_Hoarder()
        {
            var monster = MakeMonster(new Energy_Hoarder());
            monster.Energy += 13;
            monster.EndTurn();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(2, monster.VictroyPoints);
        }

        public static string Fire_Breathing()
        {
            var left = new Monster(GameComponents){Location = Location.TokyoCity};
            var right = new Monster(GameComponents);
            var monster = MakeMonster(new Fire_Breathing());
            monster.AttackPoints += 1;
            monster.Attack();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(new dynamic[] {8, left.Health, 9, right.Health});
        }

        public static string Friend_Of_Children()
        {
            var monster = MakeMonster(new Friend_Of_Children());
            monster.Energy += 1;
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(2, monster.Energy);
        }

        public static string Herbivore()
        {
            var monster = MakeMonster(new Herbivore());
            monster.Attack();
            GameComponents.Monsters.Clear();
            return UnitTests.Compare(1, monster.VictroyPoints);
        }
    }
}