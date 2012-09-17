using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PoorEngine.GameComponents
{
    public class Input : GameComponent
    {
        public KeyboardState CurrentKeyboardState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public MouseState LastMouseState;

        private Point _lastMouseLocation;

        private Vector2 _mouseMoved;
        public Vector2 MouseMoved
        {
            get { return _mouseMoved; }
        }

        public Input(Game game)
            : base(game)
        {
            Enabled = true;
        }

        /// <summary>
        /// Reads the latest state of the keyboard
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();

            _mouseMoved = new Vector2(LastMouseState.X - CurrentMouseState.X, LastMouseState.Y - CurrentMouseState.Y);
            _lastMouseLocation = new Point(CurrentMouseState.X, CurrentMouseState.Y);
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if newly pressed, false otherwise</returns>
        public bool IsNewKeyPress(Keys key)
        {
            bool status = (CurrentKeyboardState.IsKeyDown(key) &&
                LastKeyboardState.IsKeyUp(key));
            if (status)
                return status;
            return status;
        }

        /// <summary>
        /// Check for a "menu up" input action on keyboard
        /// </summary>
        public bool MenuUp
        {
            get { return IsNewKeyPress(Keys.Up); }
        }

        /// <summary>
        /// Check for a "menu down" input action on keyboard
        /// </summary>
        public bool MenuDown
        {
            get { return IsNewKeyPress(Keys.Down); }
        }

        /// <summary>
        /// Checks for a "menu select" input action on keyboard
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                    IsNewKeyPress(Keys.Enter);
            }
        }

        /// <summary>
        /// Checks for a "menu cancel" input action on keyboard
        /// </summary>
        public bool MenuCancel
        {
            get { return IsNewKeyPress(Keys.Escape); }
        }

        /// <summary>
        /// Checks for a "pause the game" action on keyboard
        /// </summary>
        public bool PauseGame
        {
            get { return IsNewKeyPress(Keys.Escape); }
        }

    }
}
