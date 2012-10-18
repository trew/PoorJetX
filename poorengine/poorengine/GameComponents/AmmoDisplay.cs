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

        }
    }
}
