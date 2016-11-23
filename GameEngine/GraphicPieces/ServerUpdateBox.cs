using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.GraphicPieces

{
    class ServerUpdateBox
    {
        private readonly GraphicsDevice graphicsDevice;
        private const int Width = 300;
        private const int Height = 150;
        private SpriteFont _font;
        private readonly Texture2D _backgroundRect;
        private readonly Vector2 _positionVector;
        private List<string> _stringList;
        private int LineSpacing = 20;

        public ServerUpdateBox(GraphicsDevice gD, SpriteFont font)
        {
            graphicsDevice = gD;
            _font = font;
            _backgroundRect = GetBackground();
            _positionVector = new Vector2(100, 100);
            _stringList = new List<string>();
        }

        private Texture2D GetBackground()
        {
            var bg = new Texture2D(graphicsDevice, Width, Height, false, SurfaceFormat.Color);
            var colorData = new Color[Width * Height];
            for (var i = 0; i < Width * Height; i++)
            {
                colorData[i] = Color.Black;
            }
            bg.SetData(colorData);
            return bg;
        }

        public void UpdateList(List<string> newList)
        {
            _stringList = newList;
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(_backgroundRect, _positionVector, Color.Black);

            var textPos = _positionVector;
            foreach (var line in _stringList)
            {
                textPos.Y = textPos.Y + LineSpacing;
                sB.DrawString(_font, line, textPos, Color.WhiteSmoke);
            }
        }
    }
}