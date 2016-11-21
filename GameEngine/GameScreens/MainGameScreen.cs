using System.Collections.Generic;
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
        }

        public override void Update(GameTime gameTime)
        {
            if (GameStateController.GameOver)
            {
                _textPrompts.Clear();
                _textPrompts.Add(new TextBlock("GameOver", new List<string>() { "Game Over" }));
                return;
            }
            UpdatePositions();
            UpdateGraphicsPieces();

            _localPlayerState = MonsterController.State(_localPlayer);
            if (_localPlayerState == State.StartOfTurn) { _gameState = GameState.StartTurn; }

            switch (_gameState)
            {
                case GameState.StartTurn:
                    StartingTurn();
                    break;
                case GameState.Rolling:
                    Rolling();
                    break;
                case GameState.Waiting:
                    break;
                default:
                    Console.Write("switch hit default.");
                    break;
            }

            if (Engine.InputManager.KeyPressed(Keys.P))
            {
                ScreenManager.AddScreen(new PauseMenu());
            }

           // _localPlayerState = MonsterController.State(_localPlayer);
          //  if (_localPlayerState == State.StartOfTurn) StartingTurn(); // Setup To Roll
          //  else if (_localPlayerState == State.Rolling) Rolling();

            //if (GameStateController.IsCurrent) { Console.WriteLine("Is Current: True"); }

            /*
            if (GameStateController.IsCurrent)
            {
                _localPlayerState = MonsterController.State(_localPlayer);
                if (_localPlayerState == State.StartOfTurn) StartingTurn(); // Setup To Roll
                else Rolling(); // Function for Rolling
                //else BuyCardPrompt();

            }
            else // Local Player is not Current.
            {
                //Console.WriteLine("Not Current! (╯°□°）╯︵ ┻━┻");
                if(_localMonster.CanYield) AskYield();
            }
            */

            base.Update(gameTime);
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
            _diceRow.Clear();
            base.UnloadAssets();
        }

        #region GameStateFunctions

        private void StartingTurn()
        {
            _diceRow.Clear();
            _textPrompts.Clear();

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
                _diceRow.AddDice(DiceController.GetDice());
                _diceRow.Hidden = false;
            }
        }

        private void Rolling()
        {
            _diceRow.Hidden = false;
            if (MonsterController.RollsRemaining(_localPlayer) == 0 || Engine.InputManager.KeyPressed(Keys.E))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.EndRolling());
                EndTurn();
            }

            if (Engine.InputManager.KeyPressed(Keys.R))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.Roll());
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

        private void EndTurn()
        {
            ServerClasses.Client.SendActionPacket(GameStateController.EndTurn());
            _gameState = GameState.Waiting;
            _diceRow.Clear();
            _diceRow.Hidden = true;
            _textPrompts.Clear();
            //TODO currently seeing how it works starting next turn automatically at end of dice rolls
            //            ServerClasses.Client.SendActionPacket(GameStateController.StartTurn());
        }

        private void AskYield()
        {
            _textPrompts.Clear();
            var s = new List<string> {MonsterController.Name(_localPlayer) + ": Yield? Y/N"};
            _textPrompts.Add(new TextBlock("YieldPrompt", s));

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

        private void BuyCardPrompt()
        {
            _textPrompts.Clear();
            var s = new List<string> {MonsterController.Name(_localPlayer) + ": Buy Cards? Y/N"};
            _textPrompts.Add(new TextBlock("BuyCardsPrompt", s));

            if (Engine.InputManager.KeyPressed(Keys.Y))
            {
                //Create a int reference variable
                //Engine.AddScreen(new BuyCards(KoTGame.CardsForSale, _currentMonster.Energy, ref variable));
                //depending on ref variable send action to buy that card
                EndTurn();
            }
            if (Engine.InputManager.KeyPressed(Keys.N))
            {
                EndTurn();
            }
        }

        #endregion

        #region PrivateHelpers

        private void UpdatePositions()
        {
            ScreenLocations.Update();
            foreach (var tp in _textPrompts)
            {
                tp.Position = ScreenLocations.GetPosition(tp.Name);
            }
            _diceRow.setPosition(ScreenLocations.GetPosition("DicePos"));
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
            var mon = MonsterController.GetById(_localPlayer);
            var monsterList = new List<Monster> { mon };
            mon = mon.Next;
            while (mon != _localMonster)
            {
                monsterList.Add(mon);
                mon = mon.Next;
            }
            return monsterList;
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

        #endregion

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
