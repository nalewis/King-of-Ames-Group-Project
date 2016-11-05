using GameEngine.DiceGraphics;
using GamePieces.Dice;
using Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace GameEngine {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        private const int PlayerBlockLength = 300;
        private const int PlayerBlockHeight = 200;
        private const int DefaultPadding = 10;

        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        public static Dictionary<string, Texture2D> TextureList;
        public static Dictionary<string, SpriteFont> FontList;
        public static Dictionary<string, Vector2> PositionList;

        private DiceRow _diceRow;

        private List<PlayerBlock> _pBlocks;
        private List<TextPrompt> _textPrompts;

        private int _screenWidth;
        private int _screenHeight;

        KeyboardState _freshKeyboardState;
        KeyboardState _oldKeyboardState;
        MouseState _freshMouseState;
        MouseState _oldMouseState;

        bool _firstUpdate;

        public Engine() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            graphics.PreferredBackBufferHeight = 720; //1080
            graphics.PreferredBackBufferWidth = 1280; //1920
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            TextureList = new Dictionary<string, Texture2D>();
            FontList = new Dictionary<string, SpriteFont>();

            _screenWidth = graphics.GraphicsDevice.Viewport.Width;
            _screenHeight = graphics.GraphicsDevice.Viewport.Height;

            PositionList = InitializePositions();

            _diceRow = new DiceRow(PositionList["DicePos"]);
            _pBlocks = new List<PlayerBlock>();
            _textPrompts = new List<TextPrompt>();

            _firstUpdate = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            LoadFonts();

            _pBlocks = InitializePlayerBlocks();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            _screenWidth = graphics.GraphicsDevice.Viewport.Width;
            _screenHeight = graphics.GraphicsDevice.Viewport.Height;
            _freshKeyboardState = Keyboard.GetState();
            _freshMouseState = Mouse.GetState();

            if (_freshKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (_firstUpdate)
            {
                GamePieces.Session.Game.StartTurn();

                _diceRow.AddDice(DiceController.GetDice());

                _firstUpdate = false;
            }

            if (_freshMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var ds in _diceRow.DiceSprites)
                {
                    if (ds.mouseOver(_freshMouseState))
                    {
                        ds.Click();
                    }
                }
            }

            if (_freshKeyboardState.IsKeyDown(Keys.Space) && _oldKeyboardState.IsKeyUp(Keys.Space))
            {
                DiceController.Roll();
            }

            foreach(var ds in _diceRow.DiceSprites)
            {
                ds.Update();
            }

            foreach (var pb in _pBlocks)
            {
                pb.Update();
            }


            _oldMouseState = _freshMouseState;
            _oldKeyboardState = _freshKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach(var pb in _pBlocks)
            {
                pb.Draw(_spriteBatch);
            }

            foreach (var ds in _diceRow.DiceSprites)
            {
                ds.Draw(_spriteBatch);
            }

            foreach(var tp in _textPrompts)
            {
                tp.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

   
        ////////Content Helpers Below//////////

        private void AddTexture(string filePath, string name)
        {
            var toAdd = Content.Load<Texture2D>(filePath);
            TextureList.Add(name, toAdd);
        }

        private void AddFont(string filePath, string name)
        {
            var toAdd = Content.Load<SpriteFont>(filePath);
            FontList.Add(name, toAdd);
        }

        private void LoadTextures()
        {
            //Load monster sprites
            AddTexture("monsterTextures\\cthulhu", "cthulhu");

            //Load dice sprites
            AddTexture("diceTextures\\dice1", "dice1");
            AddTexture("diceTextures\\dice2", "dice2");
            AddTexture("diceTextures\\dice3", "dice3");
            AddTexture("diceTextures\\diceAttack", "diceAttack");
            AddTexture("diceTextures\\diceHealth", "diceHealth");
            AddTexture("diceTextures\\diceEnergy", "diceEnergy");
        }

        private void LoadFonts()
        {
            AddFont("Fonts\\BigFont", "BigFont");
        }

        private Dictionary<string, Vector2> InitializePositions()
        {
            return new Dictionary<string, Vector2>
            {
                {"TopLeft", new Vector2(DefaultPadding, DefaultPadding)},
                {"TopCenter", new Vector2((_screenWidth/2) - (PlayerBlockLength/2), DefaultPadding)},
                {"TopRight", new Vector2(_screenWidth - DefaultPadding - PlayerBlockLength, DefaultPadding)},
                {"MidLeft", new Vector2(10, ((_screenHeight/2) - (PlayerBlockHeight/2)))},
                {"MidRight", new Vector2(_screenWidth - DefaultPadding - PlayerBlockLength, ((_screenHeight/2) - (PlayerBlockHeight/2)))},
                {"BottomCenter", new Vector2((_screenWidth/2) - (PlayerBlockLength/2), _screenHeight - PlayerBlockHeight)},
                {"TokyoCity", new Vector2() },
                {"TokyoBay", new Vector2() },
                {"DicePos", new Vector2(DefaultPadding, _screenHeight - PlayerBlockHeight)}
            };
        }

        private List<PlayerBlock> InitializePlayerBlocks()
        {
            var tempTexture = TextureList["cthulhu"];
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
