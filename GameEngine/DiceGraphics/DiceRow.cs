using GamePieces.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.DiceGraphics
{
    class DiceRow
    {
        protected DiceImage[] diceRow;
        Vector2 positon;
        int padding = 75;
        int filled;
        
        public DiceRow()
        {
            diceRow = new DiceImage[6];
            positon = new Vector2();
            filled = 0;
        }
       
        public void addDie(Die die)
        {
            DiceImage di = new DiceImage(die);
            if(filled < 6)
            {
                diceRow[filled] = di;
                filled++;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            Vector2 drawPos = positon;
            foreach (DiceImage dI in diceRow)
            {
                sb.Draw(dI.getCurrentFace(), drawPos, Microsoft.Xna.Framework.Color.White);
                drawPos.X += padding;
            }
        }
        public void UpdateDice()
        {
            foreach(DiceImage dI in diceRow)
            {
                dI.updateFromDie();
            }
        }

    }
}
