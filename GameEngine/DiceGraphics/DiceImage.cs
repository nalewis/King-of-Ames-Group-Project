using GamePieces.Dice;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.DiceGraphics
{
    class DiceImage
    {
        protected Texture2D currentFace;
        protected Die die;

        public DiceImage(Die die)
        {
            this.die = die;
            updateFromDie();
            
        }

        public void updateFromDie()
        {
            switch (die.Symbol)
            {
                case Symbol.One:
                    changeFace("dice1");
                    break;

                case Symbol.Two:
                    changeFace("dice2");
                    break;

                case Symbol.Three:
                    changeFace("dice3");
                    break;

                case Symbol.Heal:
                    changeFace("diceHealth");
                    break;

                case Symbol.Attack:
                    changeFace("diceAttack");
                    break;

                case Symbol.Energy:
                    changeFace("diceEnergy");
                    break;
            }
        }

        protected void changeFace(string newFace)
        {
            Texture2D getTexture;
            Engine.textureList.TryGetValue(newFace, out getTexture);
            currentFace = getTexture;
        }

        public Texture2D getCurrentFace()
        {
            return currentFace;
        }
    }
}
