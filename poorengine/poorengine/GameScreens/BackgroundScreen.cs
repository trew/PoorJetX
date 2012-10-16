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
        const string texture = "background";
        List<Visual> clouds;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            clouds = new List<Visual>();
            TextureManager.AddTexture(new PoorTexture("Textures/cloud1"), "cloud1");
            clouds.Add(new Visual("cloud1", 1f, 1f, 10f, true, 150, new Vector2(100,500)));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(new PoorTexture("Textures/splash"), texture);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            TextureManager.RemoveTexture(texture);
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
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));
            ScreenManager.SpriteBatch.End();

            foreach (Visual v in clouds)
            {
                v.Draw(gameTime);
            }
        }
    }
}
