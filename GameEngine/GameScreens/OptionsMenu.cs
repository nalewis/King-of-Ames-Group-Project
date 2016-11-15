
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.GameScreens
{
    class OptionsMenu : GameScreen
    {
        private const int OptionPadding = 60;
        private string[] _menuItems;
        private Vector2 _position;
        private SpriteFont _font;
        private int _stateIndex;
        private int _resolutionState;

        public new bool IsPopup = true;



        public override void Update(GameTime gameTime)
        {
            if (Engine.InputManager.KeyPressed(Keys.Down))
            {
                if (_stateIndex == (_menuItems.Length - 1))
                {
                    _stateIndex = 0;
                }
                else
                {
                    _stateIndex++;
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Up))
            {
                if (_stateIndex == 0)
                {
                    _stateIndex = _menuItems.Length - 1;
                }
                else
                {
                    _stateIndex--;
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Escape))
            {
                Engine.RemoveScreen(this);
            }

            if ((Engine.InputManager.KeyPressed(Keys.Left) || Engine.InputManager.KeyPressed(Keys.Right)) && _stateIndex == 0)
            {
                if (_resolutionState == 0)
                {
                    _resolutionState = 1;
                    _menuItems[0] = "1920x1080";
                }
                else
                {
                    _resolutionState = 0;
                    _menuItems[0] = "1280x720";
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Enter))
            {
                if (_stateIndex == 1)
                {
                    Engine.ChangeResolution(_menuItems[0]);
                    Engine.RemoveScreen(this);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin();
            for (var i = 0; i < _menuItems.Length; i++)
            {
                var text = _menuItems[i];
                var pos = new Vector2(GetCenter(text, _font), _position.Y + (OptionPadding * i));
                Engine.SpriteBatch.DrawString(_font, text, pos, _stateIndex == i ? Color.Yellow : Color.Black);
            }
            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadAssets()
        {
            _menuItems = new[] { Engine.GetResolution(), "Back" };
            _font = Engine.FontList["MenuFont"];
            _position = new Vector2(200);
            _stateIndex = 0;
            _resolutionState = 0;
        }

        private static float GetCenter(string text, SpriteFont sF)
        {
            return ((float)Engine.ScreenWidth / 2) - (sF.MeasureString(text).X / 2);
        }

    }
}
