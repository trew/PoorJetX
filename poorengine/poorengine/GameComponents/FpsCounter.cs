using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PoorEngine.GameComponents
{
    public class FpsCounter : DrawableGameComponent
    {
        private float updateInterval = 0.01f;
        private float timeSinceLastUpdate = 0.0f;
        private float framecount = 0;

        protected SpriteBatch spriteBatch;
        protected SpriteFont fpsFont;

        private float fps = 0;
        /// <summary>
        /// The frames per second
        /// </summary>
        public float FPS
        {
            get { return fps; }
        }

        public FpsCounter(Game game) : 
            base(game)
        {
            Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            framecount++;
            timeSinceLastUpdate += elapsed;

            if (timeSinceLastUpdate > updateInterval)
            {
                fps = framecount / timeSinceLastUpdate; // mean fps over updateInterval
                framecount = 0;
                timeSinceLastUpdate -= updateInterval;

                if (Updated != null)
                    Updated(this, new EventArgs());
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fpsFont = Game.Content.Load<SpriteFont>("Fonts/fpsfont");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            spriteBatch.DrawString(fpsFont, this.FPS.ToString(), new Vector2(5, 5), Color.Black);

            spriteBatch.End();
        }

        /// <summary>
        /// FpsCounter Updated Event
        /// </summary>
        public event EventHandler<EventArgs> Updated;
    }
}
