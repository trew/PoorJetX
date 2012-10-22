using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.SceneObject;
using PoorEngine.Helpers;

namespace PoorEngine.GameComponents
{
    public class AmmoDisplay : DrawableGameComponent
    {
        public ProjectileWeapon ProjectileWeapon { get; set; }
        public BombWeapon BombWeapon { get; set; }

        private static Vector2 Position;
        private Stopwatch bRefillGreen;
        private Stopwatch mgRefillGreen;

        public AmmoDisplay(Game game, ProjectileWeapon prwpn, BombWeapon bmwpn)
            : base(game)
        {
            ProjectileWeapon = prwpn;
            BombWeapon = bmwpn;
            Position = new Vector2(10, 10);

            bRefillGreen = new Stopwatch();
            mgRefillGreen = new Stopwatch();
        }

        public float MGPercentage()
        {
            if (ProjectileWeapon.Refilled)
                mgRefillGreen.Restart();

            ProjectileWeapon.Refilled = false;

            return (ProjectileWeapon.AmmoCount / (float)ProjectileWeapon.MAX_BULLETS) * 100;
        }

        public float BombsPercentage()
        {
            if (BombWeapon.Refilled)
                bRefillGreen.Restart();

            BombWeapon.Refilled = false;

            return (BombWeapon.AmmoCount / (float)BombWeapon.MAX_BOMBS) * 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (bRefillGreen.ElapsedMilliseconds > 3000)
                bRefillGreen.Stop();

            if (mgRefillGreen.ElapsedMilliseconds > 3000)
                mgRefillGreen.Stop();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 mgPos = new Vector2(0, 100);
            Vector2 bombPos = new Vector2(0, 180);

            Texture2D bombBackground;
            Color bombCountColor = Color.White;

            Texture2D MGBackground;
            Color mgCountColor = Color.White;

            Texture2D greenBackground = TextureManager.GetTexture("ammo_refill").BaseTexture as Texture2D;
            
            if      (BombsPercentage() > 25f)   bombBackground = TextureManager.GetTexture("ammo_bombs").BaseTexture as Texture2D;
            else if (BombsPercentage() == 0)    bombBackground = TextureManager.GetTexture("ammo_bombs_none").BaseTexture as Texture2D;
            else
            {
                bombBackground = TextureManager.GetTexture("ammo_bombs_low").BaseTexture as Texture2D;
                bombCountColor = Color.Red;
            }


            if      (MGPercentage() > 25f)      MGBackground = TextureManager.GetTexture("ammo_mg").BaseTexture as Texture2D;
            else if (MGPercentage() == 0)       MGBackground = TextureManager.GetTexture("ammo_mg_none").BaseTexture as Texture2D;
            else                              
            { 
                MGBackground = TextureManager.GetTexture("ammo_mg_low").BaseTexture as Texture2D;
                mgCountColor = Color.Red;
            }

            float scale = 0.40f;

            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(MGBackground,mgPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(bombBackground, bombPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(MGBackground, mgPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(bombBackground, bombPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            if(mgRefillGreen.IsRunning)
                ScreenManager.SpriteBatch.Draw(greenBackground, mgPos, null, Color.White * (1 - (((float)mgRefillGreen.ElapsedMilliseconds) / 3000f)), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            if (bRefillGreen.IsRunning)
                ScreenManager.SpriteBatch.Draw(greenBackground, bombPos, null, Color.White * (1 - (((float)bRefillGreen.ElapsedMilliseconds) / 3000f)), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.End();

            Text.DrawText(ScreenManager.Cartoon14regular, ProjectileWeapon.AmmoCount.ToString(), mgCountColor,   mgPos   + new Vector2(40, 23), 1f);
            Text.DrawText(ScreenManager.Cartoon14regular, BombWeapon.AmmoCount.ToString(),       bombCountColor, bombPos + new Vector2(40, 23), 1f);

        }
    }
}
