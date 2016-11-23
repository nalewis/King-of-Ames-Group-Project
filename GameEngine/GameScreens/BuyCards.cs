
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GamePieces.Cards;
using GamePieces.Cards.Deck.Discard;
using GamePieces.Cards.Deck.Keep;
using GamePieces.Session;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.GameScreens
{
    class BuyCards : GameScreen
    {
        private const int OptionPadding = 60;
        private readonly string[] _cardList;
        private readonly SpriteFont _font;
        private int _energy;
        private int _selected;
        private Vector2 _position;
        public new bool IsPopup = true;

        public BuyCards(string[] cardList, int energy)
        {
            _cardList = cardList;
            _energy = energy;
            _font = Engine.FontList["MenuFont"];
            _selected = 0;
            _position = new Vector2(200);
        }

        public override void Update(GameTime gameTime)
        {
            if (Engine.InputManager.KeyPressed(Keys.Down))
            {
                if (_selected == (_cardList.Length - 1))
                {
                    _selected = 0;
                }
                else
                {
                    _selected++;
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Up))
            {
                if (_selected == 0)
                {
                    _selected = _cardList.Length - 1;
                }
                else
                {
                    _selected--;
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Enter))
            {
                MainGameScreen.cardScreenChoice = _selected;
                ScreenManager.RemoveScreen(this);
            }
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var energyPos = new Vector2(_position.X, _position.Y - 25);
            Engine.SpriteBatch.Begin();
            Engine.SpriteBatch.DrawString(_font, "Current Energy: " + _energy, energyPos, Color.Blue);
            for (var i = 0; i < _cardList.Length; i++)
            {
                var text = _cardList[i];
                var pos = new Vector2(GetCenter(text, _font), _position.Y + (OptionPadding * i));
                Engine.SpriteBatch.DrawString(_font, text, pos, _selected == i ? Color.Yellow : Color.Black);
            }

            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }

        private static float GetCenter(string text, SpriteFont sF)
        {
            return ((float)Engine.ScreenWidth / 2) - (sF.MeasureString(text).X / 2);
        }

    }
}
