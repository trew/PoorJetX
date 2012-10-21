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
    public class Visual : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private float constantSpeed;
        private bool repeatable;
        private int repeatMargin;
        private Vector2 appear;
        private float scale;

        float xbloodyhell;

        /// <summary>
        /// Visual texture
        /// </summary>
        /// <param name="textureName">Name of the texture</param>
        /// <param name="Z">Depth of the visual object.</param>
        public Visual(String textureName, float scale, float Z, float constantSpeed, bool repeatable, int repeatMargin, Vector2 appear):
            base(textureName)
        {
            this.Z = Z;
            this.constantSpeed = constantSpeed;
            this.repeatable = repeatable;
            this.repeatMargin = repeatMargin;
            this.appear = appear;
            this.scale = scale;
            xbloodyhell = 0;
        }

        public void Draw(GameTime gameTime)
        {
            //if (CameraManager.Camera.Pos.X < appear.X) return;
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            ScreenManager.SpriteBatch.Begin();

            int scaleCompensation = texture.Height - (int)(texture.Height * scale);

            float y = GameHelper.ScreenHeight - texture.Height - (CameraManager.Camera.Pos.Y / Z) - appear.Y + scaleCompensation;
            if (repeatable)
            {
                // Draw the object if within screenwidth, one or more times depending on repeatMargin
                for (int i = -1; i <= (int)(GameHelper.ScreenWidth / (texture.Width + repeatMargin)) + 1; i++)
                {
                    Position = new Vector2(xbloodyhell + appear.X + i * (texture.Width + repeatMargin) - (int)CameraManager.Camera.Pos.X % ((texture.Width + repeatMargin) * Z) / Z,
                                            y);
                    ScreenManager.SpriteBatch.Draw(texture, Position, null, Color.White, 0f, new Vector2(), scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                Position = new Vector2(appear.X - (int)CameraManager.Camera.Pos.X % (texture.Width * Z) / Z, y);
                ScreenManager.SpriteBatch.Draw(texture, Position, Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            xbloodyhell += constantSpeed;
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            if (Math.Abs(xbloodyhell) > texture.Width)
            {
                if (xbloodyhell < 0)
                    xbloodyhell += texture.Width + repeatMargin;
                else
                    xbloodyhell -= texture.Width + repeatMargin;
            }
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/" + TextureName), TextureName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }
    }
}
