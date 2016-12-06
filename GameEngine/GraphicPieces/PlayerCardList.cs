using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GraphicPieces
{
    class PlayerCardList
    {
        private const int width = 500;
        private const int height = 500;
        private readonly SpriteFont _font;
        private readonly Texture2D _background;
        public Vector2 BoxPosition;
        public List<string> _stringList;
        private const int LineSpacing = 20;
        public bool Hidden;

        public PlayerCardList()
        {
            _font = Engine.FontList["updateFont"];
            BoxPosition = Vector2.Zero;
            Hidden = true;
            _background = GetBackground();
        }

        private static Texture2D GetBackground()
        {
            var bg = new Texture2D(Engine.GraffixMngr.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            var colorData = new Color[width * height];
            for (var i = 0; i < width * height; i++)
            {
                colorData[i] = Color.Black;
            }
            bg.SetData(colorData);
            return bg;
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(_background, BoxPosition, Color.Black);
            var textPos = BoxPosition;
            foreach (var line in _stringList)
            {
                sB.DrawString(_font, line, textPos, Color.White);
                textPos.Y = textPos.Y + LineSpacing;
            }
        }

    }
}
