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

        public AmmoDisplay(Game game, ProjectileWeapon prwpn, BombWeapon bmwpn)
            : base(game)
        {
            ProjectileWeapon = prwpn;
            BombWeapon = bmwpn;
            Position = new Vector2(10, 10);
        }

        public float MGPercentage()
        {
            return (ProjectileWeapon.AmmoCount / (float)ProjectileWeapon.MAX_BULLETS) * 100;
        }

        public float BombsPercentage()
        {
            return (BombWeapon.AmmoCount / (float)BombWeapon.MAX_BOMBS) * 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            EngineManager.Debug.Print("BOMBSPERCENTAGE: " + BombsPercentage());
            EngineManager.Debug.Print("MGPERCENTAGE: " + MGPercentage());

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

            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(MGBackground,mgPos, null, Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(bombBackground, bombPos, null, Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.End();

            Text.DrawText("cartoon14", ProjectileWeapon.AmmoCount.ToString(), mgCountColor,   mgPos   + new Vector2(40, 20), 1f);
            Text.DrawText("cartoon14", BombWeapon.AmmoCount.ToString(),       bombCountColor, bombPos + new Vector2(40, 20), 1f);


            /*
            Texture2D bulletTex = TextureManager.GetTexture("bullet").BaseTexture as Texture2D;
            Texture2D bombTex = TextureManager.GetTexture("bomb").BaseTexture as Texture2D;
            ScreenManager.SpriteBatch.Begin();

            // Set position for bullet-ammo-draw
            Vector2 drawPos = new Vector2(10, 10);
            for (int i = 0; i < ProjectileWeapon.BulletCount; i++)
            {
                drawPos.Y += 3;
                if (i % 100 == 0)
                {
                    drawPos.X += 5;
                    drawPos.Y = 10;
                }
                ScreenManager.SpriteBatch.Draw(bulletTex, drawPos, Color.White);
            }

            // Set position for bomb-draw
            drawPos.X = 30;
            drawPos.Y = 10;
            for (int i = 0; i < BombWeapon.BombCount; i++)
            {
                ScreenManager.SpriteBatch.Draw(bombTex, drawPos, Color.White);
                drawPos.Y += 10;
            }

            ScreenManager.SpriteBatch.End();
            */

        }
    }
}
