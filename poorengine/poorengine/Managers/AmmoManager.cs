using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace PoorEngine.Managers
{
    public class AmmoManager : GameComponent
    {
        const int MAXBULLETS = 200;
        const int MAXBOMBS = 15;
        const int BULLETSPERSECOND = 30;

        public static void Reset()
        {
            bulletCount = MAXBULLETS;
            bombCount = MAXBOMBS;
            reloadTimer.Reset();
            reloadTimer.Stop();
        }

        public static int MaxBullets
        {
            get { return MAXBULLETS; }
        }

        public static int MaxBombs
        {
            get { return MAXBOMBS; }
        }

        public static int BPS
        {
            get { return BULLETSPERSECOND; }
        }

        private static int bulletCount;
        public static int BulletCount
        {
            get { return bulletCount; }
        }

        private static int bombCount;
        public static int BombCount
        {
            get { return bombCount; }
        }

        private static Stopwatch reloadTimer;
        private static Vector2 lastBulletPos;
        private static Vector2 lastBombPos;
        private static Vector2 Position;

        public AmmoManager(Game game)
            : base(game)
        {
            Position = new Vector2(10, 10);
            bulletCount = MAXBULLETS;
            bombCount = MAXBOMBS;
            reloadTimer = new Stopwatch();
            reloadTimer.Start();
        }


        public static bool fireBullet()
        {
            if (!reloadTimer.IsRunning) { reloadTimer.Restart(); }

            if (reloadTimer.ElapsedMilliseconds > 1000 / BULLETSPERSECOND)
            {
                if (bulletCount > 0)
                {
                    bulletCount = Math.Max(bulletCount - 1, 0);
                    reloadTimer.Restart();
                    return true;
                }
            }

            return false;
        }

        public static bool dropBomb()
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

        public static void addBombs(int x)
        {
            bombCount = Math.Min(bombCount + x, MAXBOMBS);
        }

        public static void addBullets(int x)
        {
            bulletCount = Math.Min(bulletCount + x, MAXBOMBS);
        }

        public static Vector2 getLastBombPos()
        {
            return lastBombPos;
        }

        public static Vector2 getLastBulletPos()
        {
            return lastBulletPos;
        }

        public static void Update(GameTime gameTime)
        {
            
        }

    }
}
