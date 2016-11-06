using System.Collections.Generic;
using Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GameEngine.GraphicPieces;

namespace GameEngine.GameScreens
{
    class MainGameScreen : GameScreen
    {
        private List<PlayerBlock> _pBlocks;
        private List<TextPrompt> _textPrompts;
        public static Dictionary<string, Vector2> PositionList { get; private set; }
        private DiceRow _diceRow;
        private bool _firstUpdate = true;

        private const int PlayerBlockLength = 300;
        private const int PlayerBlockHeight = 200;
        private const int DefaultPadding = 10;

        public override void Update(GameTime gameTime)
        {
            CalculatePositions();
            
            if (_firstUpdate)
            {
                GamePieces.Session.Game.StartTurn();
                _diceRow.AddDice(DiceController.GetDice());
                _textPrompts.Add(new TextPrompt("Some Text", PositionList["TextPrompt1"]));
                _firstUpdate = false;
            }

            if (Engine.InputManager.KeyPressed(Keys.P))
            {
                Engine.AddScreen(new PauseMenu());
            }

            if (Engine.InputManager.LeftClick())
            {
                foreach (var ds in _diceRow.DiceSprites)
                {
                    if (ds.MouseOver(Engine.InputManager.FreshMouseState))
                    {
                        ds.Click();
                    }
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Space))
            {
                DiceController.Roll();
            }

            foreach (var ds in _diceRow.DiceSprites)
            {
                ds.Update();
            }

            foreach (var pb in _pBlocks)
            {
                pb.Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin();
            foreach (var pb in _pBlocks)
            {
                pb.Draw(Engine.SpriteBatch);
            }

            foreach (var ds in _diceRow.DiceSprites)
            {
                ds.Draw(Engine.SpriteBatch);
            }

            foreach (var tp in _textPrompts)
            {
                tp.Draw(Engine.SpriteBatch);
            }
            Engine.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void LoadAssets()
        {
            _textPrompts = new List<TextPrompt>();
            PositionList = CalculatePositions();
            _diceRow = new DiceRow(PositionList["DicePos"]);
            _pBlocks = InitializePlayerBlocks();
            base.LoadAssets();
        }

        public override void UnloadAssets()
        {
            _pBlocks.Clear();
            _textPrompts.Clear();
            PositionList.Clear();
            _diceRow.Clear();
            base.UnloadAssets();
        }

        private static Dictionary<string, Vector2> CalculatePositions()
        {
            var width = Engine.GraffixMngr.GraphicsDevice.Viewport.Width;
            var height = Engine.GraffixMngr.GraphicsDevice.Viewport.Height;
            return new Dictionary<string, Vector2>
            {
                {"TopLeft", new Vector2(DefaultPadding, DefaultPadding)},
                {"TopCenter", new Vector2((width/2) - (PlayerBlockLength/2), DefaultPadding)},
                {"TopRight", new Vector2(width - DefaultPadding - PlayerBlockLength, DefaultPadding)},
                {"MidLeft", new Vector2(10, ((height/2) - (PlayerBlockHeight/2)))},
                {"MidRight", new Vector2(width - DefaultPadding - PlayerBlockLength, ((height/2) - (PlayerBlockHeight/2)))},
                {"BottomCenter", new Vector2((width/2) - (PlayerBlockLength/2), height - PlayerBlockHeight)},
                {"TokyoCity", new Vector2() },
                {"TokyoBay", new Vector2() },
                {"DicePos", new Vector2(DefaultPadding, height - PlayerBlockHeight)},
                {"TextPrompt1", new Vector2(400, 400) }
            };
        }

        private static List<PlayerBlock> InitializePlayerBlocks()
        {
            var tempTexture = Engine.TextureList["cthulhu"];
            var monList = GamePieces.Session.Game.Monsters;
            var toReturn = new List<PlayerBlock>();

            switch (monList.Count)
            {
                case 2:
                    toReturn.Add(new PlayerBlock(tempTexture, "BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopCenter", monList[1]));
                    break;
                case 3:
                    toReturn.Add(new PlayerBlock(tempTexture, "BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopRight", monList[2]));
                    break;
                case 4:
                    toReturn.Add(new PlayerBlock(tempTexture, "TopLeft", monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopRight", monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidLeft", monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidRight", monList[3]));
                    break;
                case 5:
                    toReturn.Add(new PlayerBlock(tempTexture, "BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopRight", monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidLeft", monList[3]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidRight", monList[4]));
                    break;
                case 6:
                    toReturn.Add(new PlayerBlock(tempTexture, "BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopCenter", monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, "TopRight", monList[3]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidLeft", monList[4]));
                    toReturn.Add(new PlayerBlock(tempTexture, "MidRight", monList[5]));
                    break;
                default:
                    Console.WriteLine("Something went wrong.");
                    break;
            }

            return toReturn;
        }
    }
}
