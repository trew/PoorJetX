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

            TextureManager.AddTexture(new PoorTexture("Textures/menu/"+ background), background);
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

            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(background).BaseTexture as Texture2D,
                                            fullscreen,
                                            new Color(fade, fade, fade));

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f;

            // Modify the alpha to fade text out during transitions.
            Color color = Color.White;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);

            // Draw text, centered on the middle of each line.e

            float x = GameHelper.HalfScreenWidth;
            float y = GameHelper.HalfScreenHeight;

            string string1 = "VICTORY!";
            string[] lines = new string[]{ "Nice flyin'!", "If you enjoyed the game.. play it again!"};

            Vector2 origin = ScreenManager.Cartoon24.MeasureString(string1) / 2;
            Text.DrawText(ScreenManager.Cartoon24, string1, Color.Black, Color.White, 1f, scale, 0f, new Vector2(x, y -200), origin, false);

            for (int i = 0; i < lines.Length; i++)
            {
                origin = ScreenManager.Cartoon18.MeasureString(lines[i]) / 2;
                Text.DrawText(ScreenManager.Cartoon18, lines[i], Color.Black, Color.White, 1f, 1f, 0f, new Vector2(x, y - 100 + i * 35), origin, false);
            }

            ScreenManager.SpriteBatch.End();

        }
    }
}
