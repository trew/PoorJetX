using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoorEngine.Interfaces;
using PoorEngine.Textures;
using PoorEngine.Managers;
using PoorEngine.GameComponents;
using PoorEngine.Helpers;
using PoorEngine.SceneObject;

namespace PoorJetX.GameScreens
{
    public class BriefingScreen : GameScreen
    {
        SpriteFont _font;
        GameScreen[] _screensToLoad;
        private int _currentLevelNumber;

        public static void Load(int levelNumber, params GameScreen[] screensToLoad)
        {
            if (screensToLoad.Length > 0)
            {
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }
            }
            BriefingScreen briefingscreen = new BriefingScreen(levelNumber, screensToLoad);

            ScreenManager.AddScreen(briefingscreen);
        }


        private BriefingScreen(int levelNumber) : this(levelNumber, null) { }
        /// <summary>
        /// Constructor
        /// </summary>
        private BriefingScreen(int levelNumber, GameScreen[] screensToLoad)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            _currentLevelNumber = levelNumber;
            _screensToLoad = screensToLoad;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _font = ScreenManager.Cartoon24;

            LevelManager.Load(_currentLevelNumber);
            LevelManager.CurrentLevel.Load();

            //TextureManager.AddTexture(new PoorTexture("Textures/menu/"+ background), background);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            //TextureManager.RemoveTexture(background);
        }

        public override void HandleInput(Input input)
        {
            base.HandleInput(input);

            if (input.IsNewKeyPress(Keys.Enter))
            {
                ScreenManager.RemoveScreen(this);
                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = EngineManager.Device.Viewport;

            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            ScreenManager.SpriteBatch.Begin();

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f;

            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);

            // Draw text, centered on the middle of each line.e
            Vector2 origin = _font.MeasureString("BRIEFING") / 2;

            float x = GameHelper.HalfScreenWidth;
            float y = GameHelper.HalfScreenHeight;
            Text.DrawText(_font, "BRIEFING", Color.Black, Color.White, 1f, scale, 0f, new Vector2(x, y), origin, false);
            Text.DrawTextCentered(_font, "Get ready to roll!", Color.White, y + 40, 1f);

            ScreenManager.SpriteBatch.End();

        }
    }
}
