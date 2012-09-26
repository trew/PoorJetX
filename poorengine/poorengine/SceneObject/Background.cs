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

        /// <summary>
        /// Background texture
        /// </summary>
        /// <param name="backgroundName">Name of the texture</param>
        /// <param name="moveRatio">Ratio of which the background is moving compared to player</param>
        public Background(String backgroundName, float moveRatio)
        {
            this.backgroundName = backgroundName;
            this.moveRatio = moveRatio;
        }

        public void Draw(GameTime gameTime) 
        {
            Texture2D texture = TextureManager.GetTexture(backgroundName).BaseTexture as Texture2D;

            ScreenManager.SpriteBatch.Begin();

                // Draw the background enough times to make one at the left of the viewport and one to the right
                for (int i = -1; i <= (int)(EngineManager.Device.Viewport.Width / texture.Width) + 1; i++)
                {
                    Position = new Vector2(i * texture.Width - (int)CameraManager.Camera.Pos.X % (texture.Width * moveRatio) / moveRatio,
                                           EngineManager.Device.Viewport.Height - texture.Height);
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
