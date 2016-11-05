using GamePieces.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    internal class PlayerBlock
    {
        private Texture2D PlayerPortrait { get; }

        private string PlayerName { get; }

        private Monster Monster { get; }

        Vector2 _position;
        Vector2 _nameTextPos;
        Vector2 _healthTextPos;
        Vector2 _energyTextPos;
        Vector2 _pointsTextPos;

        private const int TextLimit = 10;
        private const int Padding = 10;
        private const int YPad = 25;

        public PlayerBlock(Texture2D texture, Vector2 pos, Monster mon)
        {
            PlayerPortrait = texture;
            _position = pos;
            Monster = mon;
            PlayerName = mon.Name;
            SetTextPositions();
        }

        protected void SetTextPositions()
        {
            _nameTextPos = new Vector2(_position.X, _position.Y + PlayerPortrait.Height + Padding);
       
            _healthTextPos = new Vector2(_position.X + PlayerPortrait.Width + Padding, _position.Y); ;
            _energyTextPos = new Vector2(_position.X + PlayerPortrait.Width + Padding, _position.Y + YPad);
            _pointsTextPos = new Vector2(_position.X + PlayerPortrait.Width + Padding, _position.Y + 2*YPad);
        }


        public void Update()
        {
            SetTextPositions();
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteFont font;
            Engine.FontList.TryGetValue("BigFont", out font);

            sb.Draw(PlayerPortrait, _position, Color.White);

            sb.DrawString(font, PlayerName.Length < TextLimit ? PlayerName : PlayerName.Substring(0, TextLimit),
                _nameTextPos, Color.Red);

            sb.DrawString(font, "Health: " + Monster.Health, _healthTextPos, Color.Blue);
            sb.DrawString(font, "Energy: " + Monster.Energy, _energyTextPos, Color.Blue);
            sb.DrawString(font, "Points: " + Monster.VictroyPoints, _pointsTextPos, Color.Blue);
        }

    }
}
