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

        private const string backgroundLayer1 = "layer-1";

        public Background()
        {
            Position = new Vector2(0, 0);
        }

        public void Draw(GameTime gameTime) 
        {
            Texture2D texture = TextureManager.GetTexture(backgroundLayer1).BaseTexture as Texture2D;
            Position = new Vector2(0, EngineManager.Device.Viewport.Height - texture.Height);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture, Position, Color.White);
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/layer-1"), backgroundLayer1);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(backgroundLayer1);
        }

        /*
         * 
         * private void DrawBG(SpriteBatch sb, Texture2D texture,float yPos, float moveRatio)
        {
            sb.Draw(texture,
                        new Vector2(-texture.Width - (((int)camera1.Pos.X % (texture.Width * moveRatio)) / moveRatio), yPos),
                        Color.AliceBlue);

            for (int i = 0; i <= (int)(screenWidth / texture.Width) + 1; i++)
            {
                sb.Draw(texture, 
                        new Vector2(i*texture.Width-(((int)camera1.Pos.X % (texture.Width * moveRatio)) / moveRatio), yPos), 
                        Color.AliceBlue);
            }
        }
         * 
         */
    }
}
