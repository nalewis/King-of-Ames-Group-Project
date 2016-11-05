using GamePieces.Dice;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameEngine.DiceGraphics
{
    class DiceRow
    {
        private List<DiceSprite> diceSprites;
        Vector2 position;
        int padding = 75;

        public DiceRow(Vector2 pos)
        {
            diceSprites = new List<DiceSprite>();
            position = pos;
        }
       
        public void addDie(Die die, int index)
        {
            Vector2 dicePos = new Vector2(position.X + (diceSprites.Count * padding), position.Y);
            DiceSprite di = new DiceSprite(die, dicePos, index);
            diceSprites.Add(di);
        }

        public List<DiceSprite> getDiceSprites()
        {
            return diceSprites;
        }

        public void clear()
        {
            diceSprites.Clear();
        }
    }
}
