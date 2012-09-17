using System;
using System.Collections.Generic;
using System.Text;
using PoorEngine.GameComponents;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PoorJetX.GameScreens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class MessageBoxScreen : GameScreen
    {
        private const string texture = "gradient";
        private string _message;

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message)
            : this(message, true)
        { }

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usageText = "Enter = ok, Esc = cancel";

            if (includeUsageText)
                _message = message + "\n" + usageText;
            else
                _message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/gradient"), texture);
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(Input input)
        {
            if (input.MenuSelect)
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, EventArgs.Empty);

                ExitScreen();
            }
            else if (input.MenuCancel)
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, EventArgs.Empty);

                ExitScreen();
            }
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = EngineManager.Device.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = ScreenManager.Font.MeasureString(_message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            ScreenManager.SpriteBatch.Begin();

            // Draw the background rectangle.
            ScreenManager.SpriteBatch.Draw(TextureManager.GetTexture(texture).BaseTexture as Texture2D, backgroundRectangle, color);

            // Draw the message box text.
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _message, textPosition, color);

            ScreenManager.SpriteBatch.End();
        }
    }
}