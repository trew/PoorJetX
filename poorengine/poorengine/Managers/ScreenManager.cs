using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.GameComponents;
using PoorEngine.Textures;
using System.Diagnostics;
using PoorEngine.Helpers;

namespace PoorEngine.Managers
{
    public class ScreenManager : DrawableGameComponent
    {
        private static List<GameScreen> _screens = new List<GameScreen>();
        private static List<GameScreen> _screensToUpdate = new List<GameScreen>();

        private int janitorCoffeeBreak;
        private static int tenMinutes = 2;
        public static int getJanitorBreakLength() { return tenMinutes; }
        public static void lengthenJanitorCoffeBreak()
        {
            tenMinutes++;
        }

        public static void shortenJanitorCoffeBreak()
        {
            tenMinutes = CalcHelper.Clamp(--tenMinutes, 1, 99999999);
        }

       

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public static GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }
 
        private static bool _initialized = false;
        /// <summary>
        /// Is the ScreenManagers Initialized, used for test cases and setup of Effects.
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }
 
        private static SpriteBatch _spriteBatch;
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }
 
        // Standard fonts shared between Screens
        private static SpriteFont _cartoon14;
        public static SpriteFont Cartoon14 { get { return _cartoon14; } }

        private static SpriteFont _cartoon18;
        public static SpriteFont Cartoon18 { get { return _cartoon18; } }

        private static SpriteFont _cartoon24;
        public static SpriteFont Cartoon24 { get { return _cartoon24; } }

        private static SpriteFont _cartoon14regular;
        public static SpriteFont Cartoon14regular { get { return _cartoon14regular; } }

        private static SpriteFont _cartoon18regular;
        public static SpriteFont Cartoon18regular { get { return _cartoon18regular; } }

        private static SpriteFont _cartoon24regular;
        public static SpriteFont Cartoon24regular { get { return _cartoon24regular; } }

        bool _traceEnabled = false;
        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return _traceEnabled; }
            set { _traceEnabled = value; }
        }
 
        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            Enabled = true;
            janitorCoffeeBreak = 0;
        }
 
        protected override void LoadContent()
        {
            base.LoadContent();
            
            _spriteBatch = new SpriteBatch(EngineManager.Device);
            _cartoon14 = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon14");
            _cartoon18 = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon18");
            _cartoon24 = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon24");
            _cartoon14regular = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon14regular");
            _cartoon18regular = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon18regular");
            _cartoon24regular = EngineManager.Game.Content.Load<SpriteFont>("Fonts/cartoon24regular");
 
            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }
 
        protected override void UnloadContent()
        {
            base.UnloadContent();
 
            foreach (GameScreen screen in _screens)
            {

                screen.UnloadContent();
            }
        }
 
        /// <summary>
        /// Initializes each screen and the screen manager itself.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();           
 
            _initialized = true;
        }
 
        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (janitorCoffeeBreak++ > tenMinutes)
            {
                GC.Collect();
                janitorCoffeeBreak = 0;
            }

            // Read the keyboard and gamepad.
            EngineManager.Input.Update();
 
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();
 
            foreach (GameScreen screen in _screens)
                _screensToUpdate.Add(screen);
 
            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;
 
            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];
 
                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);
 
                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
 
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(EngineManager.Input);
 
                        otherScreenHasFocus = true;
                    }
 
                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
 
            // Print debug trace?
            if (_traceEnabled)
                TraceScreens();
        }
 
        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        public static List<string> TraceScreens()
        {
            List<string> screenNames = new List<string>();
 
            foreach (GameScreen screen in _screens)
                screenNames.Add(screen.GetType().Name);

            return screenNames;
        }
 
        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
 
                screen.Draw(gameTime);
            }
 
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                screen.PostUIDraw(gameTime);
            }
        }
 
        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public static void AddScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to load content.
            _screens.Add(screen);
            if (_initialized)
            {
                screen.LoadContent();
            }
        }
 
        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public static void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
 
            if (_initialized)
            {
                screen.UnloadContent();
            }
 
            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
        }
 
        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public static void FadeBackBufferToBlack(int alpha)
        {
            Viewport viewport = EngineManager.Device.Viewport;
 
            _spriteBatch.Begin();
 
            _spriteBatch.Draw(TextureManager.GetColorTexture(Color.Black),
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             new Color(0, 0, 0, (byte)alpha));
 
            _spriteBatch.End();
        }
    }
}
