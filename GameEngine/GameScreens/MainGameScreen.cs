using System.Collections.Generic;
using Controllers;
using GameEngine.DiceGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;

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
            if (_firstUpdate)
            {
                GamePieces.Session.Game.StartTurn();
                _diceRow.AddDice(DiceController.GetDice());
                _textPrompts.Add(new TextPrompt("Some Text", PositionList["TextPrompt1"]));
                _firstUpdate = false;
            }

            if (Engine.InputManager.LeftClick())
            {
                foreach (var ds in _diceRow.DiceSprites)
                {
                    if (ds.mouseOver(Engine.InputManager.FreshMouseState))
                    {
                        ds.Click();
                    }
                }
            }

            if (Engine.InputManager.KeyPressed(Keys.Space))
            {
                //DiceController.Roll();

                Engine.AddScreen(new TestScreen());
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
            PositionList = InitializePositions();
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

        private static Dictionary<string, Vector2> InitializePositions()
        {
            return new Dictionary<string, Vector2>
            {
                {"TopLeft", new Vector2(DefaultPadding, DefaultPadding)},
                {"TopCenter", new Vector2((Engine.ScreenWidth/2) - (PlayerBlockLength/2), DefaultPadding)},
                {"TopRight", new Vector2(Engine.ScreenWidth - DefaultPadding - PlayerBlockLength, DefaultPadding)},
                {"MidLeft", new Vector2(10, ((Engine.ScreenHeight/2) - (PlayerBlockHeight/2)))},
                {"MidRight", new Vector2(Engine.ScreenWidth - DefaultPadding - PlayerBlockLength, ((Engine.ScreenHeight/2) - (PlayerBlockHeight/2)))},
                {"BottomCenter", new Vector2((Engine.ScreenWidth/2) - (PlayerBlockLength/2), Engine.ScreenHeight - PlayerBlockHeight)},
                {"TokyoCity", new Vector2() },
                {"TokyoBay", new Vector2() },
                {"DicePos", new Vector2(DefaultPadding, Engine.ScreenHeight - PlayerBlockHeight)},
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
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["BottomCenter"], monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopCenter"], monList[1]));
                    break;
                case 3:
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["BottomCenter"], monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopLeft"], monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopRight"], monList[2]));
                    break;
                case 4:
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopLeft"], monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopRight"], monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidLeft"], monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidRight"], monList[3]));
                    break;
                case 5:
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["BottomCenter"], monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopLeft"], monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopRight"], monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidLeft"], monList[3]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidRight"], monList[4]));
                    break;
                case 6:
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["BottomCenter"], monList[0]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopLeft"], monList[1]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopCenter"], monList[2]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["TopRight"], monList[3]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidLeft"], monList[4]));
                    toReturn.Add(new PlayerBlock(tempTexture, PositionList["MidRight"], monList[5]));
                    break;
                default:
                    Console.WriteLine("Something went wrong.");
                    break;
            }

            return toReturn;
        }
    }
}
