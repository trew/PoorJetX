using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class SkyGradient : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        public SkyGradient(string textureName):
            base(textureName)
        {
            Position = new Vector2(0, GameHelper.ScreenHeight - 2048);
            Z = 50;
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Texture2D textureNight = TextureManager.GetTexture("skygradient_night").BaseTexture as Texture2D;
            Rectangle rect = new Rectangle(0, 0, GameHelper.ScreenWidth, 2048);

            float y = Position.Y - (CameraManager.Camera.Pos.Y / (Z / 7) );

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(textureNight, new Vector2(0, y), rect, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(texture, new Vector2(0, y), rect, Color.White * (SceneGraphManager.TimeOfDay/100f), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/Landscape/" + TextureName), TextureName);
            TextureManager.AddTexture(new PoorTexture("Textures/Landscape/" + "skygradient_night"), "skygradient_night");
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }

    }
}
