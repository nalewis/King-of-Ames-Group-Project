/*

using GameEngine.DiceGraphics;
using GameEngine.PlayerDisplay;
using GamePieces.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameEngine {
    /// <summary>
    /// This is the main type for the engine
    /// </summary>
    public class Engine : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Dictionary<string, Texture2D> textureList;
        bool firstUpdate;
        DiceRow diceRow;
        SpriteFont font;
        Texture2D cth;

        PlayerBlock pb;

        int screenHeight;
        int screenWidth;

        public Engine() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Initialization logic for the game engine. 
        /// </summary>
        protected override void Initialize() {

            textureList = new Dictionary<string, Texture2D>();
            diceRow = new DiceRow();
            
            firstUpdate = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loadDice();
            loadMonsters();
            font = Content.Load<SpriteFont>("NewSpriteFont");
            textureList.TryGetValue("cthulhu", out cth);
            pb = new PlayerBlock(cth, font, new Vector2(100, 400));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (firstUpdate)
            {
                Controllers.Test.StartTurn();
                Controllers.Test.Roll();

                foreach (Die die in Controllers.Test.GetDice())
                {
                    diceRow.addDie(die);
                }
                firstUpdate = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                Controllers.Test.Roll();

            diceRow.UpdateDice();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            Texture2D toDraw;
            textureList.TryGetValue("dice1", out toDraw);
            spriteBatch.Begin();
            diceRow.Draw(spriteBatch);
            pb.draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void AddTexture(string filePath, string name)
        {
            Texture2D toAdd = Content.Load<Texture2D>(filePath);
            textureList.Add(name, toAdd);
        }

        protected void loadDice()
        {
            AddTexture("diceImages\\dice1", "dice1");
            AddTexture("diceImages\\dice2", "dice2");
            AddTexture("diceImages\\dice3", "dice3");
            AddTexture("diceImages\\diceAttack", "diceAttack");
            AddTexture("diceImages\\diceHealth", "diceHealth");
            AddTexture("diceImages\\diceEnergy", "diceEnergy");
        }
        protected void loadMonsters()
        {
            AddTexture("monsterImages\\cthulhu", "cthulhu");
        }
    }
}

*/