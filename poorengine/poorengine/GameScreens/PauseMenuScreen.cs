using System;
using System.Collections.Generic;
using System.Text;
using PoorEngine.GameComponents;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PoorJetX.GameScreens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    
    class PauseMenuScreen : MenuScreen
    {
        private const string texture = "gradient";

        public EventHandler<EventArgs> ExitToMenuEvent;

        public PauseMenuScreen() : this(400, 300) { }
        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public PauseMenuScreen(int width, int height)
            :base("Game Paused", width, height)
        {
            MenuEntry returnMenuEntry = new MenuEntry("Return");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit to Menu");
            MenuEntry exitGameEntry = new MenuEntry("ABORT EVERYTHING");

            returnMenuEntry.Selected += OnCancel;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ExitToMenu;
            exitGameEntry.Selected += ExitGame;

            MenuEntries.Add(returnMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
            MenuEntries.Add(exitGameEntry);

            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);
            IsPopup = true;
        }

        protected override void OnCancel()
        {
            SoundFxManager.Resume();
            base.OnCancel();
        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
        }

        void ExitToMenu(object sender, EventArgs e)
        {
            OnCancel();
            ExitToMenuEvent.Invoke(sender, e);
        }

        void ExitGame(object sender, EventArgs e)
        {
            EngineManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }
    }
}