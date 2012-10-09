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
        const int MAXBULLETS = 200;
        const int MAXBOMBS = 15;
        const int BULLETSPERSECOND = 30;

        private int bulletCount;
        private int bombCount;
        Stopwatch reloadTimer;
        Vector2 lastBulletPos;
        Vector2 lastBombPos;

        public AmmoController():
            base("")
        {
            Position = new Vector2(10, 10);
            bulletCount = MAXBULLETS;
            bombCount = MAXBOMBS;
            reloadTimer = new Stopwatch();
            reloadTimer.Start();
        }


        public bool fireBullet()
        {
            if(reloadTimer.ElapsedMilliseconds > 1000/BULLETSPERSECOND)
            {
                if (bulletCount > 0)
                {
                    bulletCount = Math.Max(bulletCount - 1, 0);
                    reloadTimer.Restart();
                    //reloadTimer.Start();
                    return true;
                }
            }

            return false;
        }

        public bool dropBomb()
        {
            if (bombCount > 0)
            {
                bombCount = Math.Max(bombCount - 1, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addBombs(int x)
        {
            bombCount = Math.Min(bombCount + x, MAXBOMBS);
        }

        public void addBullets(int x)
        {
            bulletCount = Math.Min(bulletCount + x, MAXBOMBS);
        }

        public Vector2 getLastBombPos() 
        {
            return lastBombPos;
        }

        public Vector2 getLastBulletPos() 
        {
            return lastBulletPos;
        }

        public void Update(GameTime gameTime)
        {
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
