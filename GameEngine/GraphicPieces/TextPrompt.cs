using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GraphicPieces
{
    internal class TextPrompt
    {
        private readonly string _text;
        public Vector2 Position { get; set; }

        public TextPrompt(string text, Vector2 position)
        {
            _text = text;
            Position = position;
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.FontList.TryGetValue("BigFont", out font);

            sb.DrawString(font, _text, Position, Color.White);
        }

    }
}
