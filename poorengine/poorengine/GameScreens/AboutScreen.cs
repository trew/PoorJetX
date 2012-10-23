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
    public class AboutScreen : GameScreen
    {
        GameScreen[] _screensToLoad;        

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutScreen()
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
                /*
                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }
                 * */
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
            Vector2 titleOrigin = ScreenManager.Cartoon24.MeasureString("About this and that") / 2;

            float rotation = (float)CalcHelper.DegreeToRadian(7.3);

            float titleX = GameHelper.ScreenWidth / 2.95f;
            float titleY = GameHelper.ScreenHeight / 5f;

            float storyX = GameHelper.ScreenWidth / 6f;
            float storyY = GameHelper.ScreenHeight / 4.1f;

            float objX = 1;
            float objY = 1;

            Text.DrawText(ScreenManager.Cartoon24, "About this and that", Color.Black, color, 1f, 1f, rotation, new Vector2(titleX, titleY), titleOrigin, false);

            string aboutText = "This game is made by Bjorn Ekberg and Samuel Andersson as a project in the course 'TDDD23 Design and Programming of Computer Games' at the University of Linkoping, Sweden. If you read this text, but dont know who we are, it must mean we've someway released it to the public. Damn, I hope we're rich by now!";

            // Split the text into lines
            string wrappedText = Text.WordWrap(aboutText, 40);
            string[] textSplit = wrappedText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Vector2 storyOrigin;
            int i = 0;

            // Draw text
            foreach (string line in textSplit)
            {
                storyOrigin = Vector2.Zero;
                Text.DrawText(ScreenManager.Cartoon14regular, textSplit[i], Color.Black, color, 1f, 1f, rotation, new Vector2(storyX - (i * 4.2f), storyY + (i * 25)), storyOrigin, false);
                i++;

                objX = storyX - ((i+2) * 3.4f);
                objY = storyY + ((i+2) * 25);
            }

            // Draw thanks to
            Text.DrawText(ScreenManager.Cartoon24, "Thanks to..", Color.Black, color, 1f, 1f, rotation, new Vector2(objX, objY), Vector2.Zero, false);
            string[] thanksTos = new string[] { "- Samuel, for his awesome codingskills", "- Bjorn, for spending hours in photoshop", "- Murphy's law, for making game-development", "  hillarious at times.", "- Mr Berglund, for hopefully giving us the", "  highest possible grade in this course"};

            i = 1;
            foreach (string thanksTo in thanksTos)
            {
                Text.DrawText(ScreenManager.Cartoon14regular, thanksTo, Color.Black, color, 1f, 1f, rotation, new Vector2(objX - ((i + 1) * 4.2f), objY + ((i + 1) * 25)), Vector2.Zero, false);
                i++;
            }

            Text.DrawText(ScreenManager.Cartoon24, "Press Enter/Esc to go back", Color.Black, Color.White, 1f, 1f, 0f, new Vector2(GameHelper.ScreenWidth - 550, GameHelper.ScreenHeight - 70), Vector2.Zero, false);

            ScreenManager.SpriteBatch.End();
        }
    }
}
