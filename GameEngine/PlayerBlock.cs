using GamePieces.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameEngine
{
    class PlayerBlock
    {
        Texture2D playerPortrait;

        string playerName;

        Monster monster;

        Vector2 position;
        Vector2 nameTextPos;
        Vector2 healthTextPos;
        Vector2 energyTextPos;
        Vector2 pointsTextPos;

        int textLimit = 10;

        int padding = 10;

        int yPad = 25;

        public PlayerBlock(Texture2D texture, Vector2 pos, Monster mon)
        {
            playerPortrait = texture;
            position = pos;
            monster = mon;
            playerName = mon.Name;
            setTextPositions();
        }

        protected void setTextPositions()
        {
            nameTextPos = new Vector2(position.X, position.Y + playerPortrait.Height + padding);

            healthTextPos = new Vector2(position.X + playerPortrait.Width + padding, position.Y); ;
            energyTextPos = new Vector2(position.X + playerPortrait.Width + padding, position.Y + yPad);
            pointsTextPos = new Vector2(position.X + playerPortrait.Width + padding, position.Y + 2*yPad);
        }


        public void Update()
        {

        }

        public void draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.fontList.TryGetValue("BigFont", out font);

            sb.Draw(playerPortrait, position, Color.White);

            if(playerName.Length < textLimit)
            {
                sb.DrawString(font, playerName, nameTextPos, Color.Red);
            }
            else
            {
                sb.DrawString(font, playerName.Substring(0, textLimit), nameTextPos, Color.Red);
            }
           
            sb.DrawString(font, "Health: " + monster.Health, healthTextPos, Color.Blue);
            sb.DrawString(font, "Energy: " + monster.Energy, energyTextPos, Color.Blue);
            sb.DrawString(font, "Points: " + monster.VictroyPoints, pointsTextPos, Color.Blue);
        }

    }
}
