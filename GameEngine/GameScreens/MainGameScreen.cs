﻿using System.Collections.Generic;
using Controllers;
using Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GameEngine.GraphicPieces;
using GamePieces.Monsters;

namespace GameEngine.GameScreens
{
    class MainGameScreen : GameScreen
    {
        public static ScreenLocations ScreenLocations;
        private readonly List<PlayerBlock> _pBlocks;
        private readonly List<TextBlock> _textPrompts;
        private readonly DiceRow _diceRow;

        private static int _localPlayer;
        private static Monster _localMonster;
        private State _localPlayerState;

        private GameState _gameState;

        public MainGameScreen()
        {
            ScreenLocations = new ScreenLocations();
            _textPrompts = new List<TextBlock>();
            _diceRow = new DiceRow(ScreenLocations.GetPosition("DicePos"));
            _localPlayer = User.PlayerId;
            _localMonster = MonsterController.GetById(_localPlayer);
            _pBlocks = InitializePlayerBlocks();
            _gameState = GameState.Waiting;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePositions(); //Possibly check for changes to rez before updating
            UpdateGraphicsPieces();
            _localPlayerState = MonsterController.State(_localPlayer);

            if (_localMonster.CanYield) _gameState = GameState.AskYield;

            if (_localPlayerState == State.StartOfTurn) _gameState = GameState.StartTurn;
            else if (_localPlayerState == State.Rolling) _gameState = GameState.Rolling;

            switch (_gameState)
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
                case GameState.Waiting:
                    break;
                case GameState.BuyCards:
                    // BuyScreen();
                    break;
                default:
                    throw new Exception("Haven't implemented this player state yet!");
            }

            if (Engine.InputManager.KeyPressed(Keys.P))
            {
                ScreenManager.AddScreen(new PauseMenu());
            }
            base.Update(gameTime);
        }

        private void StartPlayersTurn()
        {
            _diceRow.Clear();
            _textPrompts.Clear();
            _diceRow.AddDice(DiceController.GetDice());

            var stringList = new List<string>
            {
                "Your Turn " + MonsterController.Name(_localPlayer),
                "Press R to Roll, P for Menu, E to End Rolling",
                MonsterController.RollsRemaining(_localPlayer) + " Rolls Left!"
            };
            _textPrompts.Add(new TextBlock("RollingText", stringList));

            if (Engine.InputManager.KeyPressed(Keys.R))
            {
                _gameState = GameState.Rolling;
                ServerClasses.Client.SendActionPacket(GameStateController.Roll());
                _diceRow.Hidden = false;
            }
        }

        private void Rolling()
        {
            if (MonsterController.RollsRemaining(_localPlayer) == 0)
            {
                ServerClasses.Client.SendActionPacket(GameStateController.EndRolling());
                EndTurn();
            }

            if (Engine.InputManager.KeyPressed(Keys.E))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.EndRolling());
                EndTurn();
            }

            if (Engine.InputManager.KeyPressed(Keys.R))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.Roll());
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

            if (_textPrompts.Count > 0)
                _textPrompts.RemoveAt(_textPrompts.Count - 1);
            var sL = new List<string>()
            {
                "Your Turn " + MonsterController.Name(_localPlayer),
                "Press R to Roll, P for Menu, E to End Rolling",
                MonsterController.RollsRemaining(_localPlayer) + " Rolls Left!"
            };
            _textPrompts.Add(new TextBlock("RollingText", sL));
        }

        private void AskYield()
        {
            _textPrompts.Clear();
            var s = new List<string>();
            s.Add(MonsterController.Name(_localPlayer) + ": Yield? Y/N");
            _textPrompts.Add(new TextBlock("YieldPrompt", s)); // TODO Key and location can't be different, need to change.

            if (Engine.InputManager.KeyPressed(Keys.Y))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.Yield(_localPlayer));
                EndTurn();
            }
            else if (Engine.InputManager.KeyPressed(Keys.N))
            {
                EndTurn();
            }
        }

        /*
        private void BuyScreen()
        {
            Engine.AddScreen(new BuyCards(KoTGame.CardsForSale, _currentMonster.Energy));
        }
        */

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
            _diceRow.Clear();
            base.UnloadAssets();
        }

        private void EndTurn()
        {
            ServerClasses.Client.SendActionPacket(GameStateController.EndTurn());
            _diceRow.Clear();
            _diceRow.Hidden = true;
            _gameState = GameState.Waiting;
            //TODO this restarts the current players turn, how do we specify the next monster for current?
//            ServerClasses.Client.SendActionPacket(GameStateController.StartTurn());
        }

        private void UpdatePositions()
        {
            ScreenLocations.Update();
            foreach (var tp in _textPrompts)
            {
                tp.Position = ScreenLocations.GetPosition(tp.Name);
            }
            _diceRow.setPosition(ScreenLocations.GetPosition("DicePos"));
        }

        private static List<PlayerBlock> InitializePlayerBlocks()
        {
            var tempTexture = Engine.TextureList["cthulhu"];
            var monList = GetMonsterList();
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

        private static List<Monster> GetMonsterList()
        {
            Monster mon = MonsterController.GetById(_localPlayer);
            List<Monster> monsterList = new List<Monster>();
            monsterList.Add(mon);
            mon = mon.Next;
            while (mon != _localMonster)
            {
                monsterList.Add(mon);
                mon = mon.Next;
            }
            return monsterList;
        }

        internal enum GameState
        {
            StartTurn,
            Rolling,
            AskYieldBay,
            AskYield,
            BuyCards,
            Waiting
        }
    }
}
