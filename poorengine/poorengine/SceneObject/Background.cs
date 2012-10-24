using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class Background : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private float _height;
        private float YZ;       // For Z-movement-speed in the Y-axis.

        /// <summary>
        /// Background texture
        /// </summary>
        /// <param name="backgroundName">Name of the texture</param>
        /// <param name="Z">Ratio of which the background is moving compared to player</param>
        public Background(String backgroundName, float Z, float YZ, float Height):
            base(backgroundName)
        {
            this.Z = Z;
            this.YZ = YZ;
            this._height = Height;
        }

        public void Draw(GameTime gameTime) 
        {
            //if (CameraManager.Camera.Pos.X < appear.X) return;
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

            ScreenManager.SpriteBatch.Begin();

            // Draw the background enough times to make one at the left of the viewport and one to the right
            for (int i = -1; i <= (int)(GameHelper.ScreenWidth / texture.Width) + 1; i++)
            {
                Position = new Vector2(i * texture.Width - (int)CameraManager.Camera.Pos.X % (texture.Width * Z) / Z,
                    GameHelper.ScreenHeight - texture.Height - _height - (CameraManager.Camera.Pos.Y / YZ));
                ScreenManager.SpriteBatch.Draw(texture, Position, SceneGraphManager.TODcolor);
            }
            
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/Landscape/" + TextureName), TextureName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }
    }
}
