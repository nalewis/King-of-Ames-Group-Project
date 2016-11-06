using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.GameScreens;

namespace GameEngine {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        public static InputManager InputManager;
        public static SpriteBatch SpriteBatch;
        public static List<GameScreen> ScreenList;
        public static Dictionary<string, Texture2D> TextureList;
        public static Dictionary<string, SpriteFont> FontList;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public Engine() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            _graphics.PreferredBackBufferHeight = 720; //1080
            _graphics.PreferredBackBufferWidth = 1280; //1920
            _graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            ScreenWidth = _graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = _graphics.GraphicsDevice.Viewport.Height;

            TextureList = new Dictionary<string, Texture2D>();
            FontList = new Dictionary<string, SpriteFont>();
            InputManager = new InputManager(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            LoadFonts();

            AddScreen(new MainGameScreen());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            foreach (var screen in ScreenList)
            {
                screen.UnloadAssets();
            }
            TextureList.Clear();
            FontList.Clear();
            ScreenList.Clear();
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var index = ScreenList.Count - 1;
            while (ScreenList[index].IsPopup &&
                   ScreenList[index].IsActive)
            {
                index--;
            }

            for (var i = index; i < ScreenList.Count; i++)
            {
                ScreenList[i].Update(gameTime);
            }

            ScreenWidth = _graphics.GraphicsDevice.Viewport.Width;
            ScreenHeight = _graphics.GraphicsDevice.Viewport.Height;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var index = ScreenList.Count - 1;
            while (ScreenList[index].IsPopup)
            {
                index--;
            }

            GraphicsDevice.Clear(ScreenList[index].BackgroundColor);

            for (var i = index; i < ScreenList.Count; i++)
            {
                ScreenList[i].Draw(gameTime);
            }

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

        public static void AddScreen(GameScreen screen)
        {
            if (ScreenList == null) { ScreenList = new List<GameScreen>(); }
            ScreenList.Add(screen);
            screen.LoadAssets();
        }

        public static void RemoveScreen(GameScreen screen)
        {
            screen.UnloadAssets();
            ScreenList.Remove(screen);
            if(ScreenList.Count < 1) { AddScreen(new TestScreen());}
        }

        public static void ChangeScreens(GameScreen currentScreen, GameScreen nextScreen)
        {
            RemoveScreen(currentScreen);
            AddScreen(nextScreen);
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
