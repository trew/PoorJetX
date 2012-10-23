using System;
using System.Collections.Generic;
using System.Text;
using PoorEngine.GameComponents;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.SceneObject;

namespace PoorJetX.GameScreens
{
    abstract class MenuScreen : GameScreen
    {
        List<MenuEntry> _menuEntries = new List<MenuEntry>();
        /// <summary>
        /// Gets the list of menu entry strings, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return _menuEntries; }
        }

        int _selectedEntry = 0;
        string _menuTitle;
        Rectangle _borders;
        int _width;
        int _height;
        SpriteFont _titleFont;

        public MenuScreen(string menuTitle) : this(menuTitle, 400, 300) { }
        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle, int width, int height)
        {
            _menuTitle = menuTitle;
            _width = width;
            _height = height;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _titleFont = ScreenManager.Cartoon24;
            if (_borders == Rectangle.Empty)
            {
                _borders = TextureManager.GetCenterRectangle(_width, _height, EngineManager.Device.Viewport.Bounds);
            }
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(Input input)
        {
            // Move to the previous menu entry?
            if (input.MenuUp)
            {
                _selectedEntry--;

                if (_selectedEntry < 0)
                    _selectedEntry = _menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.MenuDown)
            {
                _selectedEntry++;

                if (_selectedEntry >= _menuEntries.Count)
                    _selectedEntry = 0;
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                OnSelectEntry(_selectedEntry);
            }
            else if (input.MenuCancel)
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Notifies derived classes that a menu entry has been chosen.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            _menuEntries[_selectedEntry].OnSelectEntry();
        }

        /// <summary>
        /// Notifies derived classes that the menu has been cancelled.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == _selectedEntry);

                _menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 position = new Vector2(_borders.X, _borders.Y + ScreenManager.Cartoon18.LineSpacing);
            if (_menuTitle != "")
            {
                position.Y += ScreenManager.Cartoon18.LineSpacing * 2;
            }
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.Y += transitionOffset * 256;
            else
                position.Y -= transitionOffset * 512;

            ScreenManager.SpriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                MenuEntry menuEntry = _menuEntries[i];

                bool isSelected = IsActive && (i == _selectedEntry);

                Rectangle borders = new Rectangle(_borders.X, (int)position.Y, _borders.Width, _borders.Height);
                menuEntry.Draw(this, borders, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);
            }

            // Draw the menu title.
            Vector2 titlePosition = new Vector2(
                    TextureManager.GetCenterX(_borders.X,
                                              _borders.Width,
                                              0), 
                    _borders.Y + _titleFont.LineSpacing);
            Vector2 titleOrigin = _titleFont.MeasureString(_menuTitle) / 2;
            Color titleColor = Color.Orange;
            titleColor.A = TransitionAlpha;
            float titleScale = 1.25f;

            if (ScreenState == ScreenState.TransitionOn && TransitionOnTime != TimeSpan.Zero)
                titlePosition.Y -= transitionOffset * 255;
            else if (ScreenState == ScreenState.TransitionOff && TransitionOffTime != TimeSpan.Zero)
                titlePosition.Y -= transitionOffset * 255;

            Text.DrawText(_titleFont, _menuTitle, Color.Black, titleColor, 1f, titleScale, 0f, titlePosition, titleOrigin, false);

            ScreenManager.SpriteBatch.End();
        }
    }
}
