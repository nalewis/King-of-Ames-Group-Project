﻿using GameEngine.GameScreens;
using GamePieces.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.GraphicPieces
{
    /// <summary>
    /// Graphics piece that diplays a PlayerBlock. This includes the Monster's sprite, as 
    /// well as any other relevant information such as Health, Energy, and Points.
    /// </summary>
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

        /// <summary>
        /// Creates a new PlayerBlock.
        /// </summary>
        /// <param name="texture">The texture for the Monster's image.</param>
        /// <param name="positionString">The key for getting position from MainGameScreen,</param>
        /// <param name="mon">The monster this PlayerBlock is used to display.</param>
        public PlayerBlock(Texture2D texture, string positionString, Monster mon)
        {
            PlayerPortrait = texture;
            _positionString = positionString;
            Monster = mon;
            DisplayPosition = MainGameScreen.ScreenLocations.GetPosition(positionString);
            PlayerName = mon.Name;
            SetTextPositions();
        }

        /// <summary>
        /// Sets the positions for all text contained in PlayerBlock.
        /// </summary>
        protected void SetTextPositions()
        {
            _nameTextPos = new Vector2(DisplayPosition.X, DisplayPosition.Y + PlayerPortrait.Height + Padding);
            _healthTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y);
            _energyTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y + YPad);
            _pointsTextPos = new Vector2(DisplayPosition.X + PlayerPortrait.Width + Padding, DisplayPosition.Y + 2*YPad);
        }

        /// <summary>
        /// Updates the PlayerBlocks location based on their location in the game.
        /// </summary>
        public void Update()
        {
            switch (Monster.Location)
            {
                case Location.TokyoCity:
                    DisplayPosition = MainGameScreen.ScreenLocations.GetPosition("TokyoCity");
                    break;
                case Location.TokyoBay:
                    DisplayPosition = MainGameScreen.ScreenLocations.GetPosition("TokyoBay");
                    break;
                case Location.Default:
                    DisplayPosition = MainGameScreen.ScreenLocations.GetPosition(_positionString);
                    break;
            }
            SetTextPositions();
        }

        /// <summary>
        /// Called to draw the PlayerBlock and it's data onto the screen.
        /// </summary>
        /// <param name="sb">The SpriteBatch doing the drawing.</param>
        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.FontList.TryGetValue("BigFont", out font);

            sb.Draw(PlayerPortrait, DisplayPosition, Color.White);
            sb.DrawString(font, PlayerName.Length < TextLimit ? PlayerName : PlayerName.Substring(0, TextLimit), _nameTextPos, Color.Red);
            sb.DrawString(font, "Health: " + Monster.Health, _healthTextPos, Color.Blue);
            sb.DrawString(font, "Energy: " + Monster.Energy, _energyTextPos, Color.Blue);
            sb.DrawString(font, "Points: " + Monster.VictoryPoints, _pointsTextPos, Color.Blue);
        }
    }
}
