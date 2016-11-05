using GamePieces.Dice;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameEngine.DiceGraphics
{
    class DiceRow
    {
        public List<DiceSprite> DiceSprites { get; }
        private Vector2 _position;
        private const int Padding = 75;

        public DiceRow(Vector2 pos)
        {
            DiceSprites = new List<DiceSprite>();
            _position = pos;
        }
       
        public void AddDice(List<Die> dL)
        {
            foreach (var die in dL)
            {
                var diePos = new Vector2(_position.X + (DiceSprites.Count * Padding), _position.Y);
                DiceSprites.Add(new DiceSprite(die, diePos, DiceSprites.Count));
            }
        }

        public void Clear()
        {
            DiceSprites.Clear();
        }
    }
}
