using GamePieces.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using colour = Microsoft.Xna.Framework.Color;

namespace GameEngine.DiceGraphics
{
    class DiceSprite
    {
        protected Texture2D currentFace;
        protected Die die;
        protected Vector2 position;
        protected int index;

        public DiceSprite(Die die, Vector2 pos, int index)
        {
            this.die = die;
            this.index = index;
            position = pos;
            Update();
        }

        public void Update()
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

        private void changeFace(string newFace)
        {
            Texture2D getTexture;
            Engine.textureList.TryGetValue(newFace, out getTexture);
            currentFace = getTexture;
        }

        public void Draw(SpriteBatch sb)
        {
            if (die.Save == true)
            {
                sb.Draw(currentFace, position, colour.Red);
            }
            else
            {
                sb.Draw(currentFace, position, colour.White);
            }
        }

        public bool mouseOver(MouseState mouse)
        {
            if (mouse.Position.X > position.X &&
                mouse.Position.X < position.X + currentFace.Width &&
                mouse.Position.Y > position.Y &&
                mouse.Position.Y < position.Y + currentFace.Height) { return true; }
            else
            {
                return false;
            }
        }

        public void Click()
        {
            if (die.Save)
            {
                Controllers.DiceController.UnSaveDie(index);
            }
            else
            {
                Controllers.DiceController.SaveDie(index);
            }
        }

    }
}
