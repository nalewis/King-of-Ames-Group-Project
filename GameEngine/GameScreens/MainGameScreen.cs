using System.Collections.Generic;
using Controllers;
using Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using GameEngine.GraphicPieces;
using GameEngine.ServerClasses;
using GamePieces.Monsters;
using GamePieces.Session;

namespace GameEngine.GameScreens
{
    class MainGameScreen : GameScreen
    {
        public static ScreenLocations ScreenLocations;
        private readonly List<PlayerBlock> _pBlocks;
        private static List<TextBlock> _textPrompts;
        private readonly DiceRow _diceRow;
        public ServerUpdateBox ServerUpdateBox;

        private static int _localPlayer;
        private static Monster _localMonster;
        private State _localPlayerState;
        private bool _firstPlay = true;
        private List<Monster> _monsterList;


        public static int cardScreenChoice = -1;

        private int RollAnimation { get; set; } = 0;
        private DiceRow RollingDice { get; }

        private static GameState _gameState = GameState.Waiting;

        public MainGameScreen()
        {
            ScreenLocations = new ScreenLocations();
            _textPrompts = new List<TextBlock>();
            _diceRow = new DiceRow(ScreenLocations.GetPosition("DicePos"));
            _localPlayer = User.PlayerId;
            _localMonster = MonsterController.GetById(_localPlayer);
            _monsterList = GetMonsterList();
            _pBlocks = GetPlayerBlocks();
            ServerUpdateBox = new ServerUpdateBox(Engine.FontList["updateFont"]);

            RollingDice = new DiceRow(ScreenLocations.GetPosition("DicePos"));
        }

        //public static void SetLocalPlayerState(int i)
        //{
        //    if(i == 0) _gameState = GameState.StartTurn;
        //}

        public override void Update(GameTime gameTime)
        {
            if (GameStateController.GameOver)
            {
                return;
            }
            if (GetMonsterList().Count != _monsterList.Count)
            {
                GetPlayerBlocks();
            }
            UpdatePositions();
            UpdateGraphicsPieces();

            //if(Client.isStart) _gameState = GameState.StartTurn;

            if (!MonsterController.IsDead(_localPlayer))
            {
                _localPlayerState = MonsterController.State(_localPlayer);
                if (_localPlayerState == State.StartOfTurn)
                    _gameState = GameState.StartTurn;
            }
            else
            {
                _gameState = GameState.IsDead;
            }

            switch (_gameState)
            {
                case GameState.StartTurn:
                    StartingTurn();
                    break;
                case GameState.Rolling:
                    Rolling();
                    break;
                case GameState.Waiting:
                    Waiting();
                    break;
                case GameState.BuyingCards:
                    BuyCardPrompt();
                    break;
                case GameState.EndingTurn:
                    EndTurn();
                    break;
                case GameState.AskYield:
                    AskYield();
                    break;
                case GameState.IsDead:
                    IsDead();
                    break;
                case GameState.EndGame:
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

        private void IsDead()
        {
            _diceRow.Hidden = true;
            _diceRow.Clear();

            RollingDice.Hidden = true;
            RollingDice.Clear();

            _textPrompts.Clear();

            _textPrompts.Add(new TextBlock("RollingText", new List<string> {
                "Your Dead."
                }));
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
            RollingDice.Clear();
            base.UnloadAssets();
        }

        #region GameStateFunctions

        private void StartingTurn()
        {
            _diceRow.Hidden = true;
            _diceRow.Clear();

            RollingDice.Hidden = true;
            RollingDice.Clear();

            _textPrompts.Clear();
            if (_firstPlay)
            {
                Engine.PlaySound("StartTurn");
                _firstPlay = false;
                Client.SendMessage(_localMonster.Name + " is starting their turn!");
            }
            _textPrompts.Add(new TextBlock("RollingText", new List<string> {
                "Your Turn " + MonsterController.Name(_localPlayer),
                "Press R to Roll, P for Menu ",
                "or E to End Rolling",
                //"Cards: " + MonsterController.Cards(_localPlayer).ToString()
                }));

            if (Engine.InputManager.KeyPressed(Keys.R) && RollAnimation <= 0)
            {   
                _gameState = GameState.Rolling;
                //Client.isStart = false;
                ServerClasses.Client.SendActionPacket(GameStateController.Roll());
                System.Threading.Thread.Sleep(500);
                _diceRow.AddDice(DiceController.GetDice());
                _diceRow.Hidden = false;

                RollingDice.AddDice(DiceController.GetDice());
                RollingDice.Hidden = false;
            }
        }

        private void Rolling()
        {
            if (MonsterController.RollsRemaining(_localPlayer) == 0 || Engine.InputManager.KeyPressed(Keys.E))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.EndRolling());
                //Buy Cards?
                _gameState = GameState.BuyingCards;
                return;
            }

            if (Engine.InputManager.KeyPressed(Keys.R) && RollAnimation <= 0)
            {
                Client.SendActionPacket(GameStateController.Roll());
                RollAnimation = 30;
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
            {
                _textPrompts.Remove(_textPrompts[_textPrompts.Count - 1]);
            }

            _textPrompts.Add(new TextBlock("RollingText", new List<string> {
                "Your Turn " + MonsterController.Name(_localPlayer),
                "Press R to Roll, P for Menu,",
                "or E to End Rolling",
                MonsterController.RollsRemaining(_localPlayer) + " Rolls Left!",
                //"Cards: " + MonsterController.Cards(_localPlayer).ToString()
                }));
        }

        private void EndTurn()
        {
            _textPrompts.Clear();
            _diceRow.Clear();
            _diceRow.Hidden = true;

            RollingDice.Clear();
            RollingDice.Hidden = true;

            _gameState = GameState.Waiting;
            ServerClasses.Client.SendActionPacket(GameStateController.EndTurn());
            ServerClasses.Client.SendActionPacket(GameStateController.StartTurn());
        }

        private void Waiting()
        {
            _textPrompts.Clear();
            //Console.WriteLine("Local Player Can Yeild: " + MonsterController.GetById(_localPlayer).CanYield);
            if (MonsterController.GetById(_localPlayer).CanYield)
            {
                _gameState = GameState.AskYield;
                AskYield();
            }
        }


        private void AskYield()
        {
            _textPrompts.Clear();
            var s = new List<string> {MonsterController.Name(_localPlayer) + ": Yield? Y/N"};
            _textPrompts.Add(new TextBlock("YieldPrompt", s));

            if (Engine.InputManager.KeyPressed(Keys.Y))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.Yield(_localPlayer));
                _gameState = GameState.Waiting;
                Waiting();

            }
            else if (Engine.InputManager.KeyPressed(Keys.N))
            {
                ServerClasses.Client.SendActionPacket(GameStateController.NoYield(_localPlayer));
                _gameState = GameState.Waiting;
                Waiting();
            }
        }

        private void BuyCardPrompt()
        {
            _textPrompts.Clear();

            var monList = GetMonsterList();
            if (monList.Any(mon => mon.CanYield)) { return; }

            _textPrompts.Add(new TextBlock("BuyCardsPrompt", new List<string>()
            {
                MonsterController.Name(_localPlayer) + ": Buy Cards? Y/N"
            }));

            if (Engine.InputManager.KeyPressed(Keys.Y))
            {
                ScreenManager.AddScreen(new BuyCards(MonsterController.GetById(_localPlayer).Energy));
                if (cardScreenChoice == -1) return;
                var cfs = CardsForSale.One;
                switch (cardScreenChoice)
                {
                    case 0:
                        cfs = CardsForSale.One;
                        break;
                    case 1:
                        cfs = CardsForSale.Two;
                        break;
                    case 2:
                        cfs = CardsForSale.Three;
                        break;
                    case -2:
                        break;
                    default:
                        Console.Out.WriteLine("Something went wrong with cardScreenChoice");
                        break;
                }
                if(cardScreenChoice >= 0) { Client.SendActionPacket(GameStateController.BuyCard(cfs)); }
                cardScreenChoice = -1; //reset choice for next time.
                _gameState = GameState.EndingTurn;
                EndTurn();
            }
            if (Engine.InputManager.KeyPressed(Keys.N))
            {
                _gameState = GameState.EndingTurn;
                EndTurn();
            }
        }

        public static void EndGame(string winner)
        {
            _textPrompts.Clear();
            _textPrompts.Add(new TextBlock("GameOver", new List<string>()
            {
                "Game Over",
                "Winner: " + winner + "!"
            }));
            _gameState = GameState.EndGame;

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
            RollingDice.setPosition(ScreenLocations.GetPosition("DicePos"));
        }

        private void UpdateGraphicsPieces()
        {
            foreach (var ds in _diceRow.DiceSprites)
                ds.Update();
            foreach (var pb in _pBlocks)
                pb.Update();
            ServerUpdateBox.UpdateList();
        }

        private void DrawGraphicsPieces()
        {
            ServerUpdateBox.Draw(Engine.SpriteBatch);
            foreach (var pb in _pBlocks)
                pb.Draw(Engine.SpriteBatch);

            if (!_diceRow.Hidden)
            {
                for (var i = 0; i < _diceRow.DiceSprites.Count; i++)
                {
                    if (RollAnimation <= 0 || _diceRow.DiceSprites[i].Save)
                    {
                        _diceRow.DiceSprites[i].Draw(Engine.SpriteBatch);
                    }
                    else
                    {
                        RollingDice.DiceSprites[i].Roll();
                        RollingDice.DiceSprites[i].Draw(Engine.SpriteBatch);
                        RollAnimation--;
                    }
                }
            }

            foreach (var tp in _textPrompts)
                tp.Draw(Engine.SpriteBatch);
        }

        private static List<Monster> GetMonsterList()
        {
            var mon = MonsterController.GetById(_localPlayer);
            var monList = new List<Monster> { mon };
            mon = mon.Next;
            while (mon != _localMonster)
            {
                monList.Add(mon);
                mon = mon.Next;
            }
            return monList;
        }

        private static List<PlayerBlock> GetPlayerBlocks()
        {
            var monList = GetMonsterList();
            var toReturn = new List<PlayerBlock>();
            switch (monList.Count)
            {
                case 2:
                    toReturn.Add(new PlayerBlock("BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock("TopCenter", monList[1]));
                    break;
                case 3:
                    toReturn.Add(new PlayerBlock("BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock("TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock("TopRight", monList[2]));
                    break;
                case 4:
                    toReturn.Add(new PlayerBlock("TopLeft", monList[0]));
                    toReturn.Add(new PlayerBlock("TopRight", monList[1]));
                    toReturn.Add(new PlayerBlock("MidLeft", monList[2]));
                    toReturn.Add(new PlayerBlock("MidRight", monList[3]));
                    break;
                case 5:
                    toReturn.Add(new PlayerBlock("BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock("TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock("TopRight", monList[2]));
                    toReturn.Add(new PlayerBlock("MidLeft", monList[3]));
                    toReturn.Add(new PlayerBlock("MidRight", monList[4]));
                    break;
                case 6:
                    toReturn.Add(new PlayerBlock("BottomCenter", monList[0]));
                    toReturn.Add(new PlayerBlock("TopLeft", monList[1]));
                    toReturn.Add(new PlayerBlock("TopCenter", monList[2]));
                    toReturn.Add(new PlayerBlock("TopRight", monList[3]));
                    toReturn.Add(new PlayerBlock("MidLeft", monList[4]));
                    toReturn.Add(new PlayerBlock("MidRight", monList[5]));
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
            AskYield,
            BuyingCards,
            Waiting,
            EndingTurn,
            EndGame,
            IsDead
        }
    }
}
