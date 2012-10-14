using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace PoorEngine.Managers
{
    public class EngineManager : PoorEngine
    {
        private static Game _game;
        /// <summary>
        /// The XNA Game
        /// </summary>
        public static Game Game
        {
            get { return _game; }
            set
            {
                _game = value;
            }
        }

        public EngineManager()
            : base("Engine")
        {
        }

        protected override void Draw(GameTime gameTime)
        {
            Device.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
