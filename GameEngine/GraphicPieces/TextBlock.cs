using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GameEngine.GraphicPieces
{
    /// <summary>
    /// Class for displaying text blocks onto the screen.
    /// Used for displaying prompts to the user
    /// </summary>
    internal class TextBlock
    {
        private const int LineSpacing = 30;
        private readonly List<string> _text;
        public string Name { get; }
        public Vector2 Position { get; set; }

        public TextBlock(string name, List<string> text, Vector2 position)
        {
            Name = name;
            _text = text;
            Position = position;
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.FontList.TryGetValue("BigFont", out font);
            var pos = Position;
            for (var i = 0; i < _text.Count; i++)
            {
                pos.Y = pos.Y + LineSpacing;
                sb.DrawString(font, _text[i], pos, Color.White);
            }
        }
    }
}
