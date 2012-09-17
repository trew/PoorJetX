using System;
using PoorEngine.Managers;
using PoorJetX.GameScreens;

namespace PoorEngine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            StartGame();
        }

        private static void StartGame()
        {
            using (EngineManager game = new EngineManager())
            {
                EngineManager.Game = game;
                SetupScene();
                game.Run();
            }
        }

        private static void SetupScene()
        {
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }
    }
#endif
}

