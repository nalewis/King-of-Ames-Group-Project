

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.PlayerDisplay
{
    class PlayerBlock
    {
        Texture2D playerPortrait;
        SpriteFont displayFont;

        Vector2 portraitPos;
        Vector2 healthTextPos;
        Vector2 energyTextPos;
        Vector2 pointsTextPos;

        int padding = 10;

        public PlayerBlock(Texture2D texture, SpriteFont font)
        {
            playerPortrait = texture;
            displayFont = font;
            portraitPos = new Vector2();
            setTextPositions();
        }

        public PlayerBlock(Texture2D texture, SpriteFont font, Vector2 pos)
        {
            playerPortrait = texture;
            displayFont = font;
            portraitPos = pos;
            setTextPositions();
        }

        protected void setTextPositions()
        {
            healthTextPos = getHealthTextPos();
            energyTextPos = getEnergyTextPos();
            pointsTextPos = getPointsTextPos();
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
                portraitPos.Y + displayFont.MeasureString("Health").Y + padding
                );
        }

        protected Vector2 getPointsTextPos()
        {
            return new Vector2(
                portraitPos.X + playerPortrait.Width + padding,
                energyTextPos.Y + displayFont.MeasureString("Energy").Y + padding
                );
        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(playerPortrait, portraitPos, Color.White);
            sb.DrawString(displayFont, "Health: " + "0", healthTextPos, Color.Blue);
            sb.DrawString(displayFont, "Energy: 0", energyTextPos, Color.Blue);
            sb.DrawString(displayFont, "Points: 0", pointsTextPos, Color.Blue);
        }
    }
}
