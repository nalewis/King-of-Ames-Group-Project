using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    class PlayerBlock
    {
        Texture2D playerPortrait;
        SpriteFont displayFont;

        string playerName;

        Vector2 portraitPos;
        Vector2 healthTextPos;
        Vector2 energyTextPos;
        Vector2 pointsTextPos;
        Vector2 nameTextPos;

        int textLimit = 10;
        string testString = "Health: 10";

        int padding = 10;

        public float X = 300;
        public float Y;

        public PlayerBlock(Texture2D texture, SpriteFont font, string name)
        {
            playerPortrait = texture;
            displayFont = font;
            playerName = name;
            portraitPos = new Vector2();
            setTextPositions();
            

        }

        public PlayerBlock(Texture2D texture, SpriteFont font, Vector2 pos, string name)
        {
            playerPortrait = texture;
            displayFont = font;
            playerName = name;
            portraitPos = pos;
            setTextPositions();
            
        }

        protected void setTextPositions()
        {
            nameTextPos = getNameTextPos();
            healthTextPos = getHealthTextPos();
            energyTextPos = getEnergyTextPos();
            pointsTextPos = getPointsTextPos();

            Y = playerPortrait.Height + displayFont.MeasureString(testString).Y + padding;

        }

        protected Vector2 getHealthTextPos()
        {
            return new Vector2(
                portraitPos.X + playerPortrait.Width + padding,
                portraitPos.Y
                );
        }

        protected Vector2 getEnergyTextPos()
        {
            return new Vector2(
                portraitPos.X + playerPortrait.Width + padding,
                healthTextPos.Y + displayFont.MeasureString("String").Y + padding
                );
        }

        protected Vector2 getPointsTextPos()
        {
            return new Vector2(
                portraitPos.X + playerPortrait.Width + padding,
                energyTextPos.Y + displayFont.MeasureString("String").Y + padding
                );
        }

        protected Vector2 getNameTextPos()
        {
            return new Vector2(
                portraitPos.X,
                portraitPos.Y + playerPortrait.Height + padding);
        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(playerPortrait, portraitPos, Color.White);
            if(playerName.Length < textLimit)
            {
                sb.DrawString(displayFont, playerName, nameTextPos, Color.Red);
            }
            else
            {
                sb.DrawString(displayFont, playerName.Substring(0, textLimit), nameTextPos, Color.Red);
            }
           
            sb.DrawString(displayFont, "Health: " + "0", healthTextPos, Color.Blue);
            sb.DrawString(displayFont, "Energy: 0", energyTextPos, Color.Blue);
            sb.DrawString(displayFont, "Points: 0", pointsTextPos, Color.Blue);
        }
    }
}
