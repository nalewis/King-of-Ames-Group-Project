using GameEngine.DiceGraphics;
using GamePieces.Dice;
using Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameEngine {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game {

        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        public static Dictionary<string, Texture2D> TextureList;
        public static Dictionary<string, SpriteFont> FontList;

        DiceRow _diceRow;

        List<PlayerBlock> _pBlocks;
        List<TextPrompt> _textPrompts;

        Vector2[] _playerPositions;

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
            _diceRow = new DiceRow(new Vector2(400, 350));

            _playerPositions = new Vector2[] {
                new Vector2(10, 10),
                new Vector2((_screenWidth/2) - (300/2), 10),
                new Vector2(_screenWidth - 10 - 300, 10),
                new Vector2(10, ((_screenHeight / 2) - (200 / 2))),
                new Vector2(_screenWidth - 10 - 300, ((_screenHeight/2) - (200/2))),
                new Vector2((_screenWidth / 2) - (300 / 2), _screenHeight - 200)

            };
            _textPrompts = new List<TextPrompt>();
            _pBlocks = new List<PlayerBlock>();

            int cnt = 0;
            foreach (var mon in GamePieces.Session.Game.Monsters)
            {
                var pb = new PlayerBlock(Content.Load<Texture2D>("monsterTextures\\cthulhu"), _playerPositions[cnt], mon);
                _pBlocks.Add(pb);
                cnt++;
            }

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

                var index = 0;
                foreach (var die in DiceController.GetDice())
                {
                    _diceRow.addDie(die, index);
                    index++;
                }

                _firstUpdate = false;
            }

            if (_freshMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var ds in _diceRow.getDiceSprites())
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

            foreach(var ds in _diceRow.getDiceSprites())
            {
                ds.Update();
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

            foreach(PlayerBlock pb in _pBlocks)
            {
                pb.Draw(_spriteBatch);
            }

            foreach (DiceSprite ds in _diceRow.getDiceSprites())
            {
                ds.Draw(_spriteBatch);
            }

            foreach(TextPrompt tp in _textPrompts)
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

    }
}
