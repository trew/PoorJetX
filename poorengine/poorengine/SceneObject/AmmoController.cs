using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using Microsoft.Xna.Framework;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;
using System.Diagnostics;

namespace PoorEngine.SceneObject
{
    public class AmmoController: PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private int bulletCount;
        private int bombCount;

        Vector2 lastBulletPos;
        Vector2 lastBombPos;

        public AmmoController():
            base("")
        {
            Position = new Vector2(10, 10);
            bulletCount = AmmoManager.BulletCount;
            bombCount = AmmoManager.BombCount;
        }

        public void Update(GameTime gameTime)
        {
            bulletCount = AmmoManager.BulletCount;
            bombCount = AmmoManager.BombCount;

            if (EngineManager.Input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.V))
            {
                ReadyToRender = !ReadyToRender;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!ReadyToRender) return;
            Texture2D bulletTex = TextureManager.GetTexture("bullet").BaseTexture as Texture2D;
            Texture2D bombTex = TextureManager.GetTexture("bomb").BaseTexture as Texture2D;
            
            ScreenManager.SpriteBatch.Begin();

            // Set position for bullet-ammo-draw
            Vector2 drawPos = new Vector2(10,10);
            for (int i = 0; i < bulletCount; i++)
            {
                drawPos.Y += 3;
                if (i % 100 == 0)
                {
                    drawPos.X += 5;
                    drawPos.Y = 10;
                }
                ScreenManager.SpriteBatch.Draw(bulletTex, drawPos, Color.White);
            }

            lastBulletPos = drawPos;

            // Set position for bomb-draw
            drawPos.X = 30;
            drawPos.Y = 10;
            for (int i = 0; i < bombCount; i++)
            {
                ScreenManager.SpriteBatch.Draw(bombTex, drawPos, Color.White);
                drawPos.Y += 10;
            }
            lastBombPos = drawPos;

            ScreenManager.SpriteBatch.End();
        }

        public void LoadContent()
        {
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
            //TextureManager.RemoveTexture(textureName);
        }
    }
}
