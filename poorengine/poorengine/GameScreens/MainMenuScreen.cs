using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
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
            : base("")
        {
            MenuEntry playGameManuEntry = new MenuEntry("Play Game");
            MenuEntry tutorialMenuEntry = new MenuEntry("Play Tutorial");
            MenuEntry highScoresMenuEntry = new MenuEntry("Highscores");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry controlsMenuEntry = new MenuEntry("Controls");
            MenuEntry aboutMenuEntry = new MenuEntry("About game");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            playGameManuEntry.Selected += PlayGameMenuEntrySelected;
            tutorialMenuEntry.Selected += TutorialMenuEntrySelected;
            highScoresMenuEntry.Selected += HighScoresMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            aboutMenuEntry.Selected += AboutMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameManuEntry);
            MenuEntries.Add(tutorialMenuEntry);
            MenuEntries.Add(highScoresMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(controlsMenuEntry);
            MenuEntries.Add(aboutMenuEntry);
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

            BriefingScreen.Load(1, new GamePlayScreen());
        }

        /// <summary>
        /// Event handler for when the Play Tutorial menu entry is selected.
        /// </summary>
        void TutorialMenuEntrySelected(object sender, EventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            BriefingScreen.Load(0, new GamePlayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            MessageBoxScreen messageBox = new MessageBoxScreen("Options is not available in this version.", false);
            ScreenManager.AddScreen(messageBox);
        }

        /// <summary>
        /// Event handler for when the Controls menu entry is selected.
        /// </summary>
        void ControlsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ControlsScreen());
        }

        /// <summary>
        /// Event handler for when the About menu entry is selected.
        /// </summary>
        void AboutMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new AboutScreen());
        }

        /// <summary>
        /// Event handler for when the Highscores meny entry is selected.
        /// </summary>
        void HighScoresMenuEntrySelected(object sender, EventArgs e)
        {
            MessageBoxScreen messageBox = new MessageBoxScreen("Highscores is not available in this version.", false);
            ScreenManager.AddScreen(messageBox);
            //ScreenManager.AddScreen(new HighscoreScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Give up already?";

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
