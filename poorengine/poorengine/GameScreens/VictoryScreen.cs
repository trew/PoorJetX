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
    public class VictoryScreen : GameScreen
    {
        const string background = "victoryscreen";
        SpriteFont _font;

        /// <summary>
        /// Constructor
        /// </summary>
        public VictoryScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _font = ScreenManager.Cartoon24;

            //TextureManager.AddTexture(new PoorTexture("Textures/menu/"+ background), background);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            //TextureManager.RemoveTexture(background);
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

        public override void HandleInput(Input input)
        {
            base.HandleInput(input);

            if (input.IsNewKeyPress(Keys.Enter))
            {
                ExitScreen();
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }

        }

        public override void Draw(GameTime gameTime)
        {
            Viewport viewport = EngineManager.Device.Viewport;

            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            byte fade = TransitionAlpha;

            ScreenManager.SpriteBatch.Begin();

/*            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(background).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));*/

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f;

            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);

            // Draw text, centered on the middle of each line.e

            Vector2 origin = _font.MeasureString("VICTORY!") / 2;

            float x = GameHelper.HalfScreenWidth;
            float y = GameHelper.HalfScreenHeight;
            ScreenManager.SpriteBatch.DrawString(_font, "VICTORY!", new Vector2(x, y), color, 0,
                                   origin, scale, SpriteEffects.None, 0);

            ScreenManager.SpriteBatch.End();

            Text.DrawText(ScreenManager.SpriteBatch, _font, "VICTORY!", Color.Black, Color.White, 1f, scale, 0f, new Vector2(x, y), origin, false); 
        }
    }
}
