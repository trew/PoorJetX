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
            _font = ScreenManager.Typewriter24;
            TextureManager.AddTexture(new PoorTexture("Textures/Menu/briefingscreen"), "briefingBG");

            LevelManager.Load(_currentLevelNumber);
            LevelManager.CurrentLevel.Load();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
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
            Texture2D backgroundTexture = TextureManager.GetTexture("briefingBG").BaseTexture as Texture2D;

            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(backgroundTexture, fullscreen, Color.White);
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);

            // Draw text, centered on the middle of each line.e
            Vector2 titleOrigin = _font.MeasureString(LevelManager.CurrentLevel.Briefing.Title) / 2;
            Vector2 storyOrigin = _font.MeasureString(LevelManager.CurrentLevel.Briefing.Story) / 2;

            float rotation = (float)CalcHelper.DegreeToRadian(7);

            float titleX = GameHelper.ScreenWidth / 2.95f;
            float titleY = GameHelper.ScreenHeight / 5f;

            float storyX = GameHelper.ScreenWidth / 3f;
            float storyY = GameHelper.ScreenHeight / 4.1f;

            float objX = GameHelper.ScreenWidth / 6.5f;
            float objY = GameHelper.ScreenHeight / 3f;

            Text.DrawText(_font, LevelManager.CurrentLevel.Briefing.Title, Color.Black, color, 1f, 1f, rotation, new Vector2(titleX, titleY), titleOrigin, false);
            Text.DrawText(_font, LevelManager.CurrentLevel.Briefing.Story, Color.Black, color, 1f, 1f, rotation, new Vector2(storyX, storyY), storyOrigin, false);

            int i = 1;
            foreach (LevelObjective obj in LevelManager.CurrentLevel.Briefing.Objectives)
            {
                i++;
                //Text.DrawTextCentered(_font, obj.Description, color, titleY + i * 40, 1f);
                Text.DrawText(ScreenManager.Typewriter18, obj.Description, Color.Black, color, 1f, 1f, rotation, new Vector2(objX, objY + i * 40), Vector2.Zero, false);
            }

            ScreenManager.SpriteBatch.End();

        }
    }
}
