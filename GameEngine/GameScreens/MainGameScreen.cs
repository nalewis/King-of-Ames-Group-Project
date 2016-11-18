using System.Collections.Generic;
using Controllers;
using Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GameEngine.GraphicPieces;
using GamePieces.Monsters;
using GamePieces.Session;

namespace GameEngine.GameScreens
{
    class MainGameScreen : GameScreen
    {
        private static Dictionary<string, Vector2> _spriteLocationList;
        private readonly List<PlayerBlock> _pBlocks;
        private readonly List<TextPrompt> _textPrompts;
        private readonly DiceRow _diceRow;

        private static int _localPlayer;
        private static Monster _localMonster;
        private State _localPlayerState;

        private GameState _gameState;

        private const int PlayerBlockLength = 300;
        private const int PlayerBlockHeight = 200;
        private const int DefaultPadding = 10;

        public MainGameScreen()
        {
            _spriteLocationList = GetSpriteLocations();
            _textPrompts = new List<TextPrompt>();
            _diceRow = new DiceRow(GetPosition("DicePos"));
            _localPlayer = User.PlayerId;
            _localMonster = MonsterController.GetById(_localPlayer);
            _pBlocks = InitializePlayerBlocks();
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePositions(); //Possibly check for changes to rez before updating
            _localPlayerState = MonsterController.State(_localPlayer);

            if (_localMonster.CanYield) _gameState = GameState.AskYield;

            if (_localPlayerState == State.Rolling) _gameState = GameState.Rolling;

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

            UpdateGraphicsPieces();
            base.Update(gameTime);
        }

        private void StartPlayersTurn()
        {
            _diceRow.Clear();
            _textPrompts.Clear();

            ServerClasses.Client.sendActionPacket(GameStateController.StartTurn());

            _diceRow.AddDice(DiceController.GetDice());
            _textPrompts.Add(new TextPrompt("Your Turn " + MonsterController.Name(_localPlayer), "TextPrompt1", _spriteLocationList["TextPrompt1"]));
            _textPrompts.Add(new TextPrompt("Press R to Roll, P for Menu", "TextPrompt2", _spriteLocationList["TextPrompt2"]));
            _textPrompts.Add(new TextPrompt(MonsterController.RollsRemaining(_localPlayer) + " Rolls Left!", "RollsLeft", _spriteLocationList["RollsLeft"]));
            _gameState = GameState.Rolling;
        }

        private void Rolling()
        {
            if (MonsterController.RollsRemaining(_localPlayer) == 0)
            {
                _diceRow.Clear();
                ServerClasses.Client.sendActionPacket(GameStateController.EndRolling());
                EndTurn();
            }

            if (Engine.InputManager.KeyPressed(Keys.R))
            {
                ServerClasses.Client.sendActionPacket(GameStateController.Roll());
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
            _textPrompts.Add(new TextPrompt(MonsterController.RollsRemaining(_localPlayer) + " Rolls Left!", "RollsLeft", _spriteLocationList["RollsLeft"]));
        }

        private void AskYield()
        {
            _textPrompts.Clear();
            _textPrompts.Add(new TextPrompt(MonsterController.Name(_localPlayer) + ": Yield? Y/N", "TextPrompt1", _spriteLocationList["TextPrompt1"]));

            if (Engine.InputManager.KeyPressed(Keys.Y))
            {
                ServerClasses.Client.sendActionPacket(GameStateController.Yield(_localPlayer));
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
            _spriteLocationList.Clear();
            _diceRow.Clear();
            base.UnloadAssets();
        }

        private void EndTurn()
        {
            ServerClasses.Client.sendActionPacket(GameStateController.EndTurn());
            _diceRow.Hidden = true;
            _gameState = GameState.Waiting;
        }

        private static Dictionary<string, Vector2> GetSpriteLocations()
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
                {"TokyoCity", new Vector2(400, 225) },
                {"TokyoBay", new Vector2(650, 300) },
                {"DicePos", new Vector2(DefaultPadding, height - PlayerBlockHeight)},
                {"TextPrompt1", new Vector2(width - 400, height - 100) },
                {"TextPrompt2", new Vector2(width - 400, height - 75) },
                {"RollsLeft", new Vector2(width - 400, height - 200) },
                {"WinText", new Vector2(width - 400, height - 100) }
            };
        }

        private void UpdatePositions()
        {
            _spriteLocationList = GetSpriteLocations();
            foreach (var tp in _textPrompts)
            {
                tp.Position = GetPosition(tp.Name);
            }
            _diceRow.setPosition(GetPosition("DicePos"));
        }

        private static List<PlayerBlock> InitializePlayerBlocks()
        {
            var tempTexture = Engine.TextureList["cthulhu"];
            var monList = getMonsterList();
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
            return _spriteLocationList[key];
        }

        private static List<Monster> getMonsterList()
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
