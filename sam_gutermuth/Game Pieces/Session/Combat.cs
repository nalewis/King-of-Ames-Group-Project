using System.Collections.Generic;
using System.Linq;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class Combat
    {
        private GameComponents GameComponents { get; }
        public List<Monster> Monsters => GameComponents.Monsters;
        public Board Board => GameComponents.Board;

        public Monster Attacker { get; set; }
        public List<Monster> Attacked = new List<Monster>();

        public Combat(GameComponents gameComponents)
        {
            GameComponents = gameComponents;
        }

        public void Attack(Monster attacker)
        {
            attacker.State = State.Attacking;
            Attacker = attacker;
            if (attacker.AttackPoints > 0)
            {
                Attacked = Monsters.Where(monster => monster.InTokyo != attacker.InTokyo && !monster.Equals(attacker)).ToList();
                Attacked.ForEach(monster => monster.Health -= attacker.AttackPoints);
                Attacked.Where(monster => monster.Health == 0).ToList().ForEach(monster => monster.Kill());
                Board.Update();
                if (!Board.TokyoCityIsOccupied || !Board.TokyoBayIsOccupied)
                {
                    Board.MoveIntoTokyo(attacker);
                }
            }
            attacker.State = State.None;
            attacker.AttackPoints = 0;
        }

        public void Reset()
        {
            Attacker = null;
            Attacked.Clear();
        }
    }
}