using GameEngine.GameScreens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine
{
    public class ScreenManager : GameComponent
    {
        public static List<GameScreen> ScreenList;

        public ScreenManager(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            AddScreen(new MainGameScreen());
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (ScreenList.Count == 0) return;
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
            base.Update(gameTime);
        }



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

        public void Unload()
        {
            foreach(var screen in ScreenList)
            {
                screen.UnloadAssets();
            }
        }

        public void Exit()
        {
            Unload();
            ScreenList.Clear();
        }
    }
}
