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
        SpriteBatch spriteBatch;

        public static Dictionary<string, Texture2D> textureList;
        public static Dictionary<string, SpriteFont> fontList;

        DiceRow diceRow;

        List<PlayerBlock> pBlocks;
        List<TextPrompt> textPrompts;

        List<GamePieces.Monsters.Monster> players;

        Vector2[] playerPositions;

        int screenWidth;
        int screenHeight;

        KeyboardState freshKeyboardState;
        KeyboardState oldKeyboardState;
        MouseState freshMouseState;
        MouseState oldMouseState;

        bool firstUpdate;

        int currentPlayerID;

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

            textureList = new Dictionary<string, Texture2D>();
            fontList = new Dictionary<string, SpriteFont>();

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            diceRow = new DiceRow(new Vector2(400, 350));

            playerPositions = new Vector2[] {
                new Vector2(10, 10),
                new Vector2((screenWidth/2) - (300/2), 10),
                new Vector2(screenWidth - 10 - 300, 10),
                new Vector2(10, ((screenHeight / 2) - (200 / 2))),
                new Vector2(screenWidth - 10 - 300, ((screenHeight/2) - (200/2))),
                new Vector2((screenWidth / 2) - (300 / 2), screenHeight - 200)

            };

            pBlocks = new List<PlayerBlock>();
            players = GamePieces.Session.Game.Monsters;
            textPrompts = new List<TextPrompt>();

            currentPlayerID = 0;

            firstUpdate = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            freshKeyboardState = Keyboard.GetState();
            freshMouseState = Mouse.GetState();

            if (freshKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (firstUpdate)
            {
                GamePieces.Session.Game.StartTurn();

                int index = 0;
                foreach (Die die in DiceController.GetDice())
                {
                    diceRow.addDie(die, index);
                    index++;
                }

                //////////////////////////////////////
                Texture2D cth;
                textureList.TryGetValue("cthulhu", out cth);
                int cnt = 0;
                foreach (GamePieces.Monsters.Monster player in players)
                {
                    PlayerBlock pb = new PlayerBlock(cth, playerPositions[cnt], player);
                    pBlocks.Add(pb);
                    cnt++;
                }
                //////////////////////////////////////

                firstUpdate = false;
            }

            if (freshMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
            {
                foreach (DiceSprite ds in diceRow.getDiceSprites())
                {
                    if (ds.mouseOver(freshMouseState))
                    {
                        ds.Click();
                    }
                }
            }

            if (freshKeyboardState.IsKeyDown(Keys.Space) && oldKeyboardState.IsKeyUp(Keys.Space))
            {
                DiceController.Roll();
            }

            foreach(DiceSprite ds in diceRow.getDiceSprites())
            {
                ds.Update();
            }


            oldMouseState = freshMouseState;
            oldKeyboardState = freshKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach(PlayerBlock pb in pBlocks)
            {
                pb.draw(spriteBatch);
            }

            foreach (DiceSprite ds in diceRow.getDiceSprites())
            {
                ds.Draw(spriteBatch);
            }

            foreach(TextPrompt tp in textPrompts)
            {
                tp.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

   
        ////////Content Helpers Below//////////

        private void AddTexture(string filePath, string name)
        {
            Texture2D toAdd = Content.Load<Texture2D>(filePath);
            textureList.Add(name, toAdd);
        }

        private void AddFont(string filePath, string name)
        {
            SpriteFont toAdd = Content.Load<SpriteFont>(filePath);
            fontList.Add(name, toAdd);
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
