using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.SceneObject
{
    public class SkyGradient : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private string textureName;

        public SkyGradient(string textureName)
        {
            this.textureName = textureName;

            Position = new Vector2(0, EngineManager.Device.Viewport.Height - 2048);
            Z = 50;
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(textureName).BaseTexture as Texture2D;
            Rectangle rect = new Rectangle(0, 0, EngineManager.Device.Viewport.Width, 2048);

            float y = Position.Y - (CameraManager.Camera.Pos.Y / (Z / 7) );

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture, new Vector2(0, y), rect,  Color.White, 0f, new Vector2(0,0), 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/" + textureName), textureName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(textureName);
        }

    }
}
