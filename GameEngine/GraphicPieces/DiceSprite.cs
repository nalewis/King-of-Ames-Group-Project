using GamePieces.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using colour = Microsoft.Xna.Framework.Color;

namespace GameEngine.GraphicPieces
{
    class DiceSprite
    {
        protected Texture2D CurrentFace;
        protected Die Die;
        protected Vector2 Position;
        protected int Index;
        private DiceRow _diceRow;

        public DiceSprite(Die die, Vector2 pos, int index, DiceRow diceRow)
        {
            Die = die;
            Index = index;
            Position = pos;
            _diceRow = diceRow;
            Update();
        }

        public void Update()
        {
            switch (Die.Symbol)
            {
                case Symbol.One:
                    ChangeFace("dice1");
                    break;

                case Symbol.Two:
                    ChangeFace("dice2");
                    break;

                case Symbol.Three:
                    ChangeFace("dice3");
                    break;

                case Symbol.Heal:
                    ChangeFace("diceHealth");
                    break;

                case Symbol.Attack:
                    ChangeFace("diceAttack");
                    break;

                case Symbol.Energy:
                    ChangeFace("diceEnergy");
                    break;
            }
        }

        private void ChangeFace(string newFace)
        {
            Texture2D getTexture;
            Engine.TextureList.TryGetValue(newFace, out getTexture);
            CurrentFace = getTexture;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(CurrentFace, Position, Die.Save ? colour.Red : colour.White);
        }

        public bool MouseOver(MouseState mouse)
        {
            return mouse.Position.X > Position.X &&
                   mouse.Position.X < Position.X + CurrentFace.Width &&
                   mouse.Position.Y > Position.Y &&
                   mouse.Position.Y < Position.Y + CurrentFace.Height;
        }

        public void Click()
        {
            if (_diceRow.Hidden) return;
            if (Die.Save)
            {
                ServerClasses.Client.sendActionPacket(Controllers.GameStateController.UnSaveDie(Index));
            }
            else
            {
                ServerClasses.Client.sendActionPacket(Controllers.GameStateController.SaveDie(Index));
            }
        }
    }
}
