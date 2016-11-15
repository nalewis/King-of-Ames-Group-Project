using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using GameEngine.GameScreens;

namespace GameEngine {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        public static GraphicsDeviceManager GraffixMngr;

        public static bool ExitGame = false;

        public static InputManager InputManager;
        public static SpriteBatch SpriteBatch;
        public static List<GameScreen> ScreenList;
        public static Dictionary<string, Texture2D> TextureList;
        public static Dictionary<string, SpriteFont> FontList;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public Engine() {
            GraffixMngr = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            GraffixMngr.PreferredBackBufferHeight = 720; //1080
            GraffixMngr.PreferredBackBufferWidth = 1280; //1920
            GraffixMngr.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            ScreenWidth = GraffixMngr.GraphicsDevice.Viewport.Width;
            ScreenHeight = GraffixMngr.GraphicsDevice.Viewport.Height;

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
            //if(InputManager.KeyPressed(Keys.Escape)) AddScreen(new PauseMenu());

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

            if (ExitGame)
                Exit();

            ScreenWidth = GraffixMngr.GraphicsDevice.Viewport.Width;
            ScreenHeight = GraffixMngr.GraphicsDevice.Viewport.Height;

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

        public static string GetResolution()
        {
            return GraffixMngr.PreferredBackBufferWidth == 1280 ? "1280x720" : "1920x1080";
        }

        public static void ChangeResolution(string res)
        {
            if (res.Equals("1280x720"))
            {
                GraffixMngr.PreferredBackBufferWidth = 1280;
                GraffixMngr.PreferredBackBufferHeight = 720;
            }
            else if (res.Equals("1920x1080"))
            {
                GraffixMngr.PreferredBackBufferWidth = 1920;
                GraffixMngr.PreferredBackBufferHeight = 1080;
            }
            GraffixMngr.ApplyChanges();
        }

        #region ContentHelpers

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
            AddFont("Fonts\\MenuFont", "MenuFont");
        }

        #endregion

        #region GameScreenHelpers

        public static void AddScreen(GameScreen newScreen)
        {
            if (ScreenList == null) { ScreenList = new List<GameScreen>(); }
            if (ScreenList.Any(screen => screen.GetType() == newScreen.GetType())) { return; }
            ScreenList.Add(newScreen);
            newScreen.LoadAssets();
        }

        public static void RemoveScreen(GameScreen screen)
        {
            screen.UnloadAssets();
            ScreenList.Remove(screen);
            if (ScreenList.Count < 1) { AddScreen(new TestScreen()); }
        }

        public static void ChangeScreens(GameScreen currentScreen, GameScreen nextScreen)
        {
            RemoveScreen(currentScreen);
            AddScreen(nextScreen);
        }

        #endregion

    }
}
