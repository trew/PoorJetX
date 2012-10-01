using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;

namespace PoorEngine.SceneObject
{
    public class Background : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {

        private string backgroundName;

        private float moveRatio;

        private bool repeatable;
        private int repeatMargin;
        private Vector2 appear;

        private bool alignToBottom;

        /// <summary>
        /// Background texture
        /// </summary>
        /// <param name="backgroundName">Name of the texture</param>
        /// <param name="moveRatio">Ratio of which the background is moving compared to player</param>
        public Background(String backgroundName, float moveRatio, bool repeatable, int repeatMargin, Vector2 appear, bool alignToBottom)
        {
            this.backgroundName = backgroundName;
            this.moveRatio = moveRatio;
            this.repeatable = repeatable;
            this.repeatMargin = repeatMargin;
            this.appear = appear;
            this.alignToBottom = alignToBottom;
        }

        public void Draw(GameTime gameTime) 
        {
            //if (CameraManager.Camera.Pos.X < appear.X) return;
            Texture2D texture = TextureManager.GetTexture(backgroundName).BaseTexture as Texture2D;

            ScreenManager.SpriteBatch.Begin();

            float y = alignToBottom ? EngineManager.Device.Viewport.Height - texture.Height - (CameraManager.Camera.Pos.Y / moveRatio) : appear.Y;
            if (repeatable)
            {
                if (appear.X > 0)
                {
                    // Do we have enough textures to fill the whole screen?
                    EngineManager.Debug.Print("Appear.X: " + appear.X);
                    EngineManager.Debug.Print("Viewport Width: " + EngineManager.Device.Viewport.Width);
                    EngineManager.Debug.Print("texture Width: " + texture.Width);
                }
                // Draw the background enough times to make one at the left of the viewport and one to the right
                for (int i = -1; i <= (int)(EngineManager.Device.Viewport.Width / texture.Width + repeatMargin) + 1; i++)
                {
                    Position = new Vector2(i * (texture.Width + repeatMargin) - (int)CameraManager.Camera.Pos.X % ((texture.Width + repeatMargin) * moveRatio) / moveRatio, y);
                    ScreenManager.SpriteBatch.Draw(texture, Position, Color.White);
                }
            }
            else
            {
                Position = new Vector2(appear.X -(int)CameraManager.Camera.Pos.X % (texture.Width * moveRatio) / moveRatio, y);
                EngineManager.Debug.Print("BG Pos: " + Position);
                ScreenManager.SpriteBatch.Draw(texture, Position, Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/" + backgroundName), backgroundName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(backgroundName);
        }
    }
}
