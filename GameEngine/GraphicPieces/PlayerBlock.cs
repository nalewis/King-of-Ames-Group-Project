﻿using GameEngine.GameScreens;
using GamePieces.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GraphicPieces
{
    internal class PlayerBlock
    {

        public Vector2 DisplayPosition { get; set; }
        private Texture2D PlayerPortrait { get; }
        private Monster Monster { get; }
        private string PlayerName { get; }
        private readonly string _positionString;

        private Vector2 _nameTextPos;
        private Vector2 _healthTextPos;
        private Vector2 _energyTextPos;
        private Vector2 _pointsTextPos;

        private const int TextLimit = 10;
        private const int Padding = 10;
        private const int YPad = 25;

        public PlayerBlock(Texture2D texture, string positionString, Monster mon)
        {
            PlayerPortrait = texture;
            _positionString = positionString;
            Monster = mon;

            DisplayPosition = MainGameScreen.GetPosition(positionString);
            PlayerName = mon.Name;
            SetTextPositions();
        }

        protected void SetTextPositions()
        {
            _nameTextPos = new Vector2(DisplayPosition.X, DisplayPosition.Y + PlayerPortrait.Height + Padding);
       
            _healthTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y);
            _energyTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y + YPad);
            _pointsTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y + 2*YPad);
        }


        public void Update()
        {
            switch (Monster.Location)
            {
                case Location.TokyoCity:
                    DisplayPosition = MainGameScreen.GetPosition("TokyoCity");
                    break;
                case Location.TokyoBay:
                    DisplayPosition = MainGameScreen.GetPosition("TokyoBay");
                    break;
                case Location.Default:
                    DisplayPosition = MainGameScreen.GetPosition(_positionString);
                    break;
            }

            SetTextPositions();
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.FontList.TryGetValue("BigFont", out font);

            sb.Draw(PlayerPortrait, DisplayPosition, Color.White);

            sb.DrawString(font, PlayerName.Length < TextLimit ? PlayerName : PlayerName.Substring(0, TextLimit),
                _nameTextPos, Color.Red);

            sb.DrawString(font, "Health: " + Monster.Health, _healthTextPos, Color.Blue);
            sb.DrawString(font, "Energy: " + Monster.Energy, _energyTextPos, Color.Blue);
            sb.DrawString(font, "Points: " + Monster.VictroyPoints, _pointsTextPos, Color.Blue);
        }
    }
}