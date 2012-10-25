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
    public class ControlsScreen : GameScreen
    {
        GameScreen[] _screensToLoad;        

        /// <summary>
        /// Constructor
        /// </summary>
        public ControlsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(0.5f);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            TextureManager.AddTexture(new PoorTexture("Textures/Menu/briefingscreen"), "briefingBG");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void HandleInput(Input input)
        {
            base.HandleInput(input);

            if (input.IsNewKeyPress(Keys.Enter)
                || input.IsNewKeyPress(Keys.Escape))
            {
                ScreenManager.RemoveScreen(this);
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
            
            float rotation = (float)CalcHelper.DegreeToRadian(7.3);

            float titleX = GameHelper.ScreenWidth / 2.95f;
            float titleY = GameHelper.ScreenHeight / 5f;

            float controlsX = GameHelper.ScreenWidth / 6f;
            float controlsY = GameHelper.ScreenHeight / 4.1f;

            // controls
            Text.DrawText(ScreenManager.Cartoon18, "Keybindings", Color.Black, color, 1f, 1f, rotation, new Vector2(controlsX, controlsY), Vector2.Zero, false);
            string[] thanksTos = new string[] { "Key Left/Right - Rotate Left/Right", "Holding Shift - Increase rotationspeed", "Z/X - Decrease/Increase Throttle", "Left Ctrl - Fire Machinegun", "Hold Spacebar - Show bombpath", "Release Spacebar - Drop bomb", "A - Pugachev's Cobra maneuver", "Escape - Pause game" };

            int i = 1;
            foreach (string thanksTo in thanksTos)
            {
                Text.DrawText(ScreenManager.Cartoon14regular, thanksTo, Color.Black, color, 1f, 1f, rotation, new Vector2(controlsX - ((i + 1) * 4.2f), controlsY + ((i + 1) * 25)), Vector2.Zero, false);
                i++;
            }

            Text.DrawText(ScreenManager.Cartoon24, "Press Enter/Esc to go back", Color.Black, Color.White, 1f, 1f, 0f, new Vector2(GameHelper.ScreenWidth - 550, GameHelper.ScreenHeight - 70), Vector2.Zero, false);

            ScreenManager.SpriteBatch.End();
        }
    }
}
