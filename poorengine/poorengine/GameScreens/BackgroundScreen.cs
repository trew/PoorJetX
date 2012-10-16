using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Textures;
using PoorEngine.Managers;
using PoorEngine.GameComponents;
using PoorEngine.SceneObject;

namespace PoorJetX.GameScreens
{
    public class BackgroundScreen : GameScreen
    {
        const string texture_bg = "background_bg";
        const string texture_fg = "background_fg";

        List<Visual> clouds;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            clouds = new List<Visual>();
            
            clouds.Add(new Visual("cloud1", 1f, 1f, 0.6f, true, 1700, new Vector2(-100, 400)));
            clouds.Add(new Visual("cloud2", 0.35f, 1f, 0.1f, true, 1500, new Vector2(500, 300)));
            clouds.Add(new Visual("cloud3", 0.5f, 1f, 0.6f, true, 2300, new Vector2(300, 250)));
            clouds.Add(new Visual("cloud4", 1f, 1f, 0.45f, true, 1850, new Vector2(-500, 500)));
            clouds.Add(new Visual("cloud5", 0.2f, 1f, 0.05f, true, 2100, new Vector2(130, 250)));
            clouds.Add(new Visual("cloud4", 0.1f, 1f, 0.15f, true, 1850, new Vector2(-100, 500)));
            clouds.Add(new Visual("cloud5", 0.07f, 1f, 0.05f, true, 2100, new Vector2(330, 370)));
            clouds.Add(new Visual("cloud4", 0.05f, 1f, 0.02f, true, 1850, new Vector2(-200, 420)));
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(new PoorTexture("Textures/splash_bg"), texture_bg);
            TextureManager.AddTexture(new PoorTexture("Textures/splash_fg"), texture_fg);

            TextureManager.AddTexture(new PoorTexture("Textures/cloud1"), "cloud1");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud2"), "cloud2");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud3"), "cloud3");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud4"), "cloud4");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud5"), "cloud5");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            TextureManager.RemoveTexture(texture_fg);
            TextureManager.RemoveTexture(texture_bg);
        }
        
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            foreach (Visual v in clouds)
            {
                v.Update(gameTime);
            }

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = EngineManager.Device.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture_bg).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));
            ScreenManager.SpriteBatch.End();

            foreach (Visual v in clouds)
            {
                v.Draw(gameTime);
            }

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture_fg).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));
            ScreenManager.SpriteBatch.End();
        }
    }
}
