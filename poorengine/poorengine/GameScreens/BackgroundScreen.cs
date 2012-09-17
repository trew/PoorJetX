using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Textures;
using PoorEngine.Managers;
using PoorEngine.GameComponents;

namespace PoorJetX.GameScreens
{
    public class BackgroundScreen : GameScreen
    {
        const string texture = "background";

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            TextureManager.AddTexture(new PoorTexture("Textures/background"), texture);
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
        }
    }
}
