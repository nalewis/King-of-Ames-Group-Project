using System.Collections.Generic;
using GamePieces.Dice;
using Microsoft.Xna.Framework;

namespace GameEngine.GraphicPieces
{
    class DiceRow
    {
        public List<DiceSprite> DiceSprites { get; }
        private Vector2 _position;
        private const int Padding = 75;

        public bool Hidden { get; set; }

        public DiceRow(Vector2 pos)
        {
            DiceSprites = new List<DiceSprite>();
            _position = pos;
            Hidden = true;
        }
       
        public void AddDice(List<Die> dL)
        {
            foreach (var die in dL)
            {
                var diePos = new Vector2(_position.X + (DiceSprites.Count * Padding), _position.Y);
                DiceSprites.Add(new DiceSprite(die, diePos, DiceSprites.Count, this));
            }
        }

        public void setPosition(Vector2 pos)
        {
            _position = pos;
        }

        public void Clear()
        {
            DiceSprites.Clear();
        }
    }
}
