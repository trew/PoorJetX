using System;
using System.Collections.Generic;
using System.Text;
using PoorEngine.Managers;
using PoorEngine.GameScreens;
using PoorEngine.GameComponents;

namespace PoorJetX.GameScreens
{
    class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// The main menu screen is the first thing displayed when the game starts up.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            MenuEntry playGameManuEntry = new MenuEntry("Play Game");
            MenuEntry highScoresMenuEntry = new MenuEntry("Highscores");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            playGameManuEntry.Selected += PlayGameMenuEntrySelected;
            highScoresMenuEntry.Selected += HighScoresMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameManuEntry);
            MenuEntries.Add(highScoresMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            ScreenManager.AddScreen(new GamePlayScreen(1));
            
            //LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        /// <summary>
        /// Event handler for when the Highscores meny entry is selected.
        /// </summary>
        void HighScoresMenuEntrySelected(object sender, EventArgs e)
        {
            //ScreenManager.AddScreen(new HighScoresScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen messageBox = new MessageBoxScreen(message);
            messageBox.Accepted += ExitMessageBoxAccepted;
            ScreenManager.AddScreen(messageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            EngineManager.Game.Exit();
        }
    }
}