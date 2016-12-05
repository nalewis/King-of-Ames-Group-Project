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
            _stringList = _stringList.Count < 5 ? Client.MessageHistory : Client.MessageHistory.GetRange(Client.MessageHistory.Count - 6, 5);
        }

        public void Draw(SpriteBatch sB)
        {
            sB.Draw(_backgroundRect, _positionVector, Color.Black);
            var textPos = _positionVector;
            var listCopy = _stringList;
            foreach (var line in listCopy)
            {
                sB.DrawString(_font, line, textPos, Color.WhiteSmoke);
                textPos.Y = textPos.Y + LineSpacing;
            }
        }
    }
}