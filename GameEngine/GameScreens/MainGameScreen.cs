﻿using System.Collections.Generic;
using Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GameEngine.GraphicPieces;
using GamePieces.Monsters;
using GamePieces.Session;
using KoTGame = GamePieces.Session.Game;

namespace GameEngine.GameScreens
{
    class MainGameScreen : GameScreen
    {
        private static Dictionary<string, Vector2> _positionList;
        private readonly List<PlayerBlock> _pBlocks;
        private readonly List<TextPrompt> _textPrompts;
        private readonly DiceRow _diceRow;

        private Monster _currentMonster;
        private int _rollsLeft;

        private GameState _gameState;

        private const int PlayerBlockLength = 300;
        private const int PlayerBlockHeight = 200;
        private const int DefaultPadding = 10;

        public MainGameScreen()
        {
            LobbyController.StartGame();
            _positionList = CalculatePositions();
            _pBlocks = InitializePlayerBlocks();
            _textPrompts = new List<TextPrompt>();
            _diceRow = new DiceRow(GetPosition("DicePos"));
            _currentMonster = KoTGame.Current;       
            _gameState = GameState.StartTurn;
        }

        public override void Update(GameTime gameTime)
        {
            if (KoTGame.Winner == null)
            {

                //Here in case they moved, but should probably change this
                CalculatePositions();

                //Make sure we're on current monster
                _currentMonster = KoTGame.Current;

                var gameState = _gameState;

                switch (gameState)
                {
                    case GameState.StartTurn:
                        StartPlayersTurn();
                        break;
                    case GameState.Rolling:
                        Rolling();
                        break;
                    case GameState.AskYield:
                        AskYield();
                        break;
                    default:
                        throw new Exception("Haven't implemented this player state yet!");
                }
            }
            else
            {
                _textPrompts.Clear();
                _textPrompts.Add(new TextPrompt(KoTGame.Winner.Name + " WINS!!", GetPosition("WinText")));
            }

            if (Engine.InputManager.KeyPressed(Keys.P))
            {
                Engine.AddScreen(new PauseMenu());
            }

            UpdateGraphicsPieces();
            base.Update(gameTime);
        }

        private void StartPlayersTurn()
        {
            _currentMonster.StartTurn();
            //Check so we don't keep adding!
            if (_textPrompts.Count != 0 || _diceRow.DiceSprites.Count != 0) return;
            _diceRow.AddDice(DiceController.GetDice());
            _textPrompts.Add(new TextPrompt("Your Turn " + _currentMonster.Name, _positionList["TextPrompt1"]));
            _textPrompts.Add(new TextPrompt("Press R to Roll, P for Menu", _positionList["TextPrompt2"]));
            _textPrompts.Add(new TextPrompt(_rollsLeft + " Rolls Left!", _positionList["RollsLeft"]));
            _gameState = GameState.Rolling;
        }

        private void Rolling()
        {
            _rollsLeft = _currentMonster.RemainingRolls;
            if (_rollsLeft == 0)
            {
                _diceRow.Clear();
                _textPrompts.Clear();
                KoTGame.EndRolling();
                DiceController.EndRolling();
                if (Board.TokyoCityIsOccupied || Board.TokyoBayIsOccupied)
                {
                    _gameState = GameState.AskYield;
                }
                else
                {
                    KoTGame.EndTurn();
                    _gameState = GameState.StartTurn;
                }
                return;
            }

            if (Engine.InputManager.KeyPressed(Keys.R))
            {
                DiceController.Roll();
                _diceRow.Hidden = false;
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

            _textPrompts.RemoveAt(_textPrompts.Count - 1);
            _textPrompts.Add(new TextPrompt(_rollsLeft + " Rolls Left!", _positionList["RollsLeft"]));
        }

        private void AskYield()
        {
            _textPrompts.Clear();
            if (Board.TokyoCityIsOccupied && Board.TokyoCity.CanYield)
            {
                _textPrompts.Add(new TextPrompt(Board.TokyoCity.Name + ": Yield? Y/N", _positionList["TextPrompt1"]));
                if (Engine.InputManager.KeyPressed(Keys.Y))
                {
                    Board.TokyoCity.Yield();
                }
                else if (Engine.InputManager.KeyPressed(Keys.N))
                {
                    _textPrompts.Clear();
                    KoTGame.EndTurn();
                    _gameState = GameState.StartTurn;
                }
            }
            else
            {
                KoTGame.EndTurn();
                _gameState = GameState.StartTurn;
            }
            /*
            if (Board.TokyoBayIsOccupied && Board.TokyoBay.CanYield)
            {
                _textPrompts.Add(new TextPrompt(Board.TokyoBay.Name + ": Yield? Y/n", _positionList["TextPrompt2"]));
                if (Engine.InputManager.KeyPressed(Keys.Y))
                {
                    Board.TokyoBay.Yield();
                }
                else if (Engine.InputManager.KeyPressed(Keys.N))
                {
                    _gameState = GameState.StartTurn;
                }
            }
            */
        }

        

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin();
            DrawGraphicsPieces();
            Engine.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void UnloadAssets()
        {
            _pBlocks.Clear();
            _textPrompts.Clear();
            _positionList.Clear();
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
                {"TokyoCity", new Vector2(490, 225) },
                {"TokyoBay", new Vector2() },
                {"DicePos", new Vector2(DefaultPadding, height - PlayerBlockHeight)},
                {"TextPrompt1", new Vector2(175, 250) },
                {"TextPrompt2", new Vector2(175, 400) },
                {"RollsLeft", new Vector2(175, 275) },
                {"WinText", new Vector2(300, 400) }
            };
        }

        private static List<PlayerBlock> InitializePlayerBlocks()
        {
            var tempTexture = Engine.TextureList["cthulhu"];
            var monList = KoTGame.Monsters;
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

        private void UpdateGraphicsPieces()
        {
            foreach (var ds in _diceRow.DiceSprites)
                ds.Update();
            foreach (var pb in _pBlocks)
                pb.Update();
        }

        private void DrawGraphicsPieces()
        {
            foreach (var pb in _pBlocks)
                pb.Draw(Engine.SpriteBatch);

            if (!_diceRow.Hidden)
            {
                foreach (var ds in _diceRow.DiceSprites)
                    ds.Draw(Engine.SpriteBatch);
            }

            foreach (var tp in _textPrompts)
                tp.Draw(Engine.SpriteBatch);
        }

        public static Vector2 GetPosition(string key)
        {
            return _positionList[key];
        }

        internal enum GameState
        {
            StartTurn,
            Rolling,
            AskYield,

        }
    }
}
