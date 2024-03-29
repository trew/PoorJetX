﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Managers;
using PoorEngine.SceneObject;

namespace PoorJetX.GameScreens
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        string text;
        SpriteFont font;
        Color textColor;
        Color selectedColor;
        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        public MenuEntry(string text)
            :this(text, Color.DarkRed, Color.Red)
        {
        }
        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, Color textColor, Color selectedColor)
        {
            this.text = text;
            this.textColor = textColor;
            this.selectedColor = selectedColor;
            this.font = ScreenManager.Cartoon24;
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            Draw(screen, new Rectangle((int)position.X, (int)position.Y, 0, 0), isSelected, gameTime);
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, Rectangle borders,
                                 bool isSelected, GameTime gameTime)
        {
            if(font == null)
                font = ScreenManager.Cartoon24;

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? selectedColor : textColor;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.

            Vector2 origin = font.MeasureString(text) / 2;

            Vector2 position;
            if (borders.Height <= 0)
                position = new Vector2(borders.X, borders.Y);
            else
            {
                float x = TextureManager.GetCenterX(borders.X, borders.Width, 0);
                position = new Vector2((int)x, borders.Y);
            }

            PoorEngine.SceneObject.Text.DrawText(
                font,
                Text,
                Color.Black,
                color,
                1f,
                scale,
                0f,
                position, 
                origin,
                false);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return font.LineSpacing;
        }
    }
}
