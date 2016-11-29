using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace GameEngine.GameScreens
{
    internal class ScreenLocations
    {
        private const int PlayerBlockLength = 350;
        private const int PlayerBlockHeight = 250;
        private const int DefaultPadding = 10;
        private Dictionary<string, Vector2> _positions;
        private int _screenWidth;
        private int _screeHeight;

        public ScreenLocations()
        {
            _screenWidth = 0;
            _screeHeight = 0;
            Update();
        }

        public void Update()
        {
            if (_screenWidth == Engine.GraffixMngr.GraphicsDevice.Viewport.Width) return;
            _screenWidth = Engine.GraffixMngr.GraphicsDevice.Viewport.Width;
            _screeHeight = Engine.GraffixMngr.GraphicsDevice.Viewport.Height;
            _positions = new Dictionary<string, Vector2>()
            {
                {"TopLeft", new Vector2(DefaultPadding, DefaultPadding)},
                {"TopCenter", new Vector2((_screenWidth/2) - (PlayerBlockLength/2), DefaultPadding)},
                {"TopRight", new Vector2(_screenWidth - DefaultPadding - PlayerBlockLength, DefaultPadding)},
                {"MidLeft", new Vector2(10, ((_screeHeight/2) - (PlayerBlockHeight/2)))},
                {"MidRight", new Vector2(_screenWidth - DefaultPadding - PlayerBlockLength, ((_screeHeight/2) - (PlayerBlockHeight/2)))},
                {"BottomCenter", new Vector2((_screenWidth/2) - (PlayerBlockLength/2), _screeHeight - PlayerBlockHeight)},
                {"TokyoCity", new Vector2(400, 225) },
                {"TokyoBay", new Vector2(650, 300) },
                {"DicePos", new Vector2(DefaultPadding, _screeHeight - PlayerBlockHeight)},
                {"TextPrompt1", new Vector2(_screenWidth - 400, _screeHeight - 100) },
                {"TextPrompt2", new Vector2(_screenWidth - 400, _screeHeight - 75) },
                {"RollsLeft", new Vector2(_screenWidth - 400, _screeHeight - 200) },
                {"WinText", new Vector2(_screenWidth - 400, _screeHeight - 100) },
                {"YieldPrompt", new Vector2(_screenWidth - 400, _screeHeight - 100) },
                {"RollingText", new Vector2(_screenWidth - 500, _screeHeight - 200) },
                {"BuyCardsPrompt", new Vector2(_screenWidth - 400, _screeHeight - 100) },
                {"GameOver", new Vector2(_screenWidth - 400, _screeHeight - 100) },
                {"ServerUpdateBox", new Vector2(10, _screeHeight - 150)  },
                {"cardList", new Vector2(_screenWidth - 100, 500)  }
            };
        }

        public Vector2 GetPosition(string key)
        {
            return _positions[key];
        }
    }
}
