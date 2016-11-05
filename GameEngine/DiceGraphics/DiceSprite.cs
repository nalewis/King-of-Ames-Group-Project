using GamePieces.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using colour = Microsoft.Xna.Framework.Color;

namespace GameEngine.DiceGraphics
{
    class DiceSprite
    {
        protected Texture2D CurrentFace;
        protected Die Die;
        protected Vector2 Position;
        protected int Index;

        public DiceSprite(Die die, Vector2 pos, int index)
        {
            Die = die;
            Index = index;
            Position = pos;
            Update();
        }

        public void Update()
        {
            switch (Die.Symbol)
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
            Engine.TextureList.TryGetValue(newFace, out getTexture);
            CurrentFace = getTexture;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(CurrentFace, Position, Die.Save ? colour.Red : colour.White);
        }

        public bool mouseOver(MouseState mouse)
        {
            if (mouse.Position.X > Position.X &&
                mouse.Position.X < Position.X + CurrentFace.Width &&
                mouse.Position.Y > Position.Y &&
                mouse.Position.Y < Position.Y + CurrentFace.Height) { return true; }
            else
            {
                return false;
            }
        }

        public void Click()
        {
            if (Die.Save)
            {
                Controllers.DiceController.UnSaveDie(Index);
            }
            else
            {
                Controllers.DiceController.SaveDie(Index);
            }
        }

    }
}
