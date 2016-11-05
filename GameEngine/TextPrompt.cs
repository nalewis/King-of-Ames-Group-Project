using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    class TextPrompt
    {
        string text;
        Vector2 position;

        public TextPrompt(string text, Vector2 position)
        {
            this.text = text;
            this.position = position;
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.fontList.TryGetValue("BigFont", out font);

            sb.DrawString(font, text, position, Color.White);
        }

    }
}
