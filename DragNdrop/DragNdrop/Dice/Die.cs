using System;

namespace GamePieces.Dice
{
    public class Die
    {
        private readonly Random Random;
        private readonly int Faces;

        public Color Color { get; }
        public Symbol Symbol { get; private set; }
        public bool Save { get; set; }

        public Die(Color color, Random random, int faces)
        {
            Color = color;
            Symbol = 0;
            Save = false;
            Random = random;
            Faces = faces;
        }

        public void Roll()
        {
            if (!Save) Symbol = (Symbol) Random.Next(0, Faces);
        }
    }
}