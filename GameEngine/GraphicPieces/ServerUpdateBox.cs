using System.Collections.Generic;
using GameEngine.GameScreens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.ServerClasses;

namespace GameEngine.GraphicPieces

{
    class ServerUpdateBox
    {
        private readonly GraphicsDevice graphicsDevice;
        private const int Width = 300;
        private const int Height = 150;
        private SpriteFont _font;
        private Texture2D _backgroundRect;
        private readonly Vector2 _positionVector;
        private List<string> _stringList;
        private int LineSpacing = 20;

        public ServerUpdateBox(SpriteFont font)
        {
            _font = font;
            _positionVector = MainGameScreen.ScreenLocations.GetPosition("ServerUpdateBox");
            _stringList = new List<string>();
            _backgroundRect = GetBackground();
        }

        private Texture2D GetBackground()
        {
            var bg = new Texture2D(Engine.GraffixMngr.GraphicsDevice,  Width, Height, false, SurfaceFormat.Color);
            var colorData = new Color[Width * Height];
            for (var i = 0; i < Width * Height; i++)
            {
                colorData[i] = Color.Black;
            }
            bg.SetData(colorData);
            return bg;
        }

        public void UpdateList()
        {
            if (Client.MessageHistory.Count < 3)
            {
                _stringList = Client.MessageHistory;
            }
            else
            {
                _stringList = Client.MessageHistory.GetRange(Client.MessageHistory.Count - 4,
                    Client.MessageHistory.Count - 1);
            }
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(_backgroundRect, _positionVector, Color.Black);
            var textPos = _positionVector;
            foreach (var line in _stringList)
            {
                sB.DrawString(_font, line, textPos, Color.WhiteSmoke);
                textPos.Y = textPos.Y + LineSpacing;
            }
        }
    }
}