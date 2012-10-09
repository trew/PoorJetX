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

        private int bullets;
        private int bombs;
        Stopwatch reloadTimer;

        public AmmoController():
            base("")
        {
            bullets = MAXBULLETS;
            bombs = MAXBOMBS;
            reloadTimer = new Stopwatch();
            reloadTimer.Start();
        }


        public bool fireBullet()
        {
            if(reloadTimer.ElapsedMilliseconds > 1000/BULLETSPERSECOND)
            {
                if (bullets > 0)
                {
                    bullets = Math.Max(bullets - 1, 0);
                    reloadTimer.Restart();
                    //reloadTimer.Start();
                    return true;
                }
            }

            return false;
        }

        public bool dropBomb()
        {
            if (bombs > 0)
            {
                bombs = Math.Max(bombs - 1, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addBombs(int x)
        {
            bombs = Math.Min(bombs + x, MAXBOMBS);
        }

        public void addBullets(int x)
        {
            bullets = Math.Min(bullets + x, MAXBOMBS);
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
           

            ScreenManager.SpriteBatch.End();
        }

        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
            //TextureManager.RemoveTexture(textureName);
        }
    }
}
