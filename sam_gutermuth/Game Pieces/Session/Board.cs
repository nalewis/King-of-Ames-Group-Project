using System;
using GamePieces.Monsters;

namespace GamePieces.Session
{
    public class Board
    {
        //Game Components
        private GameComponents GameComponents { get; }
        public int Monsters => GameComponents.Monsters.Count;
        public bool UseTokyoBay => Monsters > 4;

        //State
        public Monster TokyoCity { get; private set; }
        public Monster TokyoBay { get; private set; }

        public bool TokyoCityIsOccupied => TokyoCity != null;
        public bool TokyoBayIsOccupied => TokyoBay != null;

        public Board(GameComponents gameComponents)
        {
            GameComponents = gameComponents;
        }

        public void MoveIntoTokyo(Monster monster)
        {
            if(monster.InTokyo) return;
            if (!TokyoCityIsOccupied)
            {
                TokyoCity = monster;
                monster.Location = Location.TokyoCity;
                monster.VictroyPoints += 1;
            }
            else if (UseTokyoBay && !TokyoBayIsOccupied)
            {
                TokyoBay = monster;
                monster.Location = Location.TokyoBay;
                monster.VictroyPoints += 1;
            }
        }

        public void LeaveTokyo(Monster monster)
        {
            if (TokyoCityIsOccupied && monster.Equals(TokyoCity))
            {
                TokyoCity = null;
                monster.Location = Location.Default;
            }
            else if (TokyoBayIsOccupied && monster.Equals(TokyoBay))
            {
                TokyoBay = null;
                monster.Location = Location.Default;
            }
        }

        public void Update()
        {
            if (UseTokyoBay || !TokyoBayIsOccupied) return;
            TokyoBay.Location = Location.Default;
            TokyoBay = null;
        }

        public void Reset()
        {
            TokyoCity = TokyoBay = null;
        }
    }
}