
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GameEngine.GameScreens
{
    class PauseMenu : GameScreen
    {
        private const int OptionPadding = 60;
        private string[] _menuOptions;
        private Vector2 _position;
        private SpriteFont _font;
        private int _stateIndex;

        public new bool IsPopup = true;

        public override void Update(GameTime gameTime)
        {
            if (Engine.InputManager.KeyPressed(Keys.Down))
            {
                if (_stateIndex == (_menuOptions.Length - 1))
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
                    _stateIndex = _menuOptions.Length - 1;
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

            if (Engine.InputManager.KeyPressed(Keys.Enter))
            {
                switch (_stateIndex)
                {
                    case 0:
                        Engine.RemoveScreen(this);
                        break;
                    case 1:
                        Engine.AddScreen(new OptionsMenu());
                        break;
                    case 2:
                        Engine.ExitGame = true;
                        break;
                }
            }



            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin();
            for(var i = 0; i < _menuOptions.Length; i++)
            {
                var text = _menuOptions[i];
                var pos = new Vector2(GetCenter(text, _font), _position.Y + (OptionPadding * i));
                Engine.SpriteBatch.DrawString(_font, text, pos, _stateIndex == i ? Color.Yellow : Color.Black);
            }
            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void LoadAssets()
        {
            _menuOptions = new[] {"Resume", "Options", "Exit"};
            _font = Engine.FontList["MenuFont"];
            _position = new Vector2(200);
            _stateIndex = 0;
        }

        private static float GetCenter(string text, SpriteFont sF)
        {
            return ((float)Engine.ScreenWidth/2) - (sF.MeasureString(text).X/2);
        }
    }
}
