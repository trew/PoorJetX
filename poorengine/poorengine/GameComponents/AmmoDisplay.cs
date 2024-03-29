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
using PoorEngine.Textures;
using PoorEngine.Settings;

namespace PoorEngine.GameComponents
{
    public class AmmoDisplay
    {
        public ProjectileWeapon ProjectileWeapon { get; set; }
        public BombWeapon BombWeapon { get; set; }

        private static Vector2 Position;
        private Stopwatch bRefillGreen;
        private Stopwatch mgRefillGreen;

        private Stopwatch soundTimer;
        private bool playSound;

        public AmmoDisplay(ProjectileWeapon prwpn, BombWeapon bmwpn)
        {
            ProjectileWeapon = prwpn;
            BombWeapon = bmwpn;
            Position = new Vector2(10, 10);

            playSound = false;

            soundTimer = new Stopwatch();
            soundTimer.Start();

            bRefillGreen = new Stopwatch();
            mgRefillGreen = new Stopwatch();
        }

        public float MGPercentage()
        {
            if (ProjectileWeapon.Refilled)
            {
                mgRefillGreen.Restart();
                ProjectileWeapon.Refilled = false;
                playSound = true;
            }
            

            return (ProjectileWeapon.AmmoCount / (float)ProjectileWeapon.MAX_BULLETS) * 100;
        }

        public float BombsPercentage()
        {
            if (BombWeapon.Refilled)
            {
                bRefillGreen.Restart();
                BombWeapon.Refilled = false;
                playSound = true;
            }
            

            return (BombWeapon.AmmoCount / (float)BombWeapon.MAX_BOMBS) * 100;
        }

        public void Update(GameTime gameTime)
        {
            if (bRefillGreen.ElapsedMilliseconds > 3000)
                bRefillGreen.Stop();

            if (mgRefillGreen.ElapsedMilliseconds > 3000)
                mgRefillGreen.Stop();

            if (playSound == true && soundTimer.ElapsedMilliseconds > 350)
            {
                SoundFxLibrary.GetFx("refill").Play(SoundFxManager.GetVolume("Sound", 0.5f), 0f, -0.5f);
                soundTimer.Restart();
                playSound = false;
            }
        }

        public void LoadContent()
        {
            // For ammo-related UI
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_bombs"), "ammo_bombs");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_bombs_low"), "ammo_bombs_low");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_mg"), "ammo_mg");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_mg_low"), "ammo_mg_low");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_refill"), "ammo_refill");
        }
        public void UnloadContent()
        {
        }

        public void Draw(GameTime gameTime)
        {
            if (GameSettings.Default.ShowUI && !LevelManager.CurrentLevel.Completed)
            {
                Vector2 mgPos = new Vector2(0, 100);
                Vector2 bombPos = new Vector2(0, 180);

                Texture2D bombBackground;
                Color bombCountColor = Color.White;

                Texture2D MGBackground;
                Color mgCountColor = Color.White;

                Texture2D greenBackground = TextureManager.GetTexture("ammo_refill").BaseTexture as Texture2D;

                if (BombsPercentage() > 25f) bombBackground = TextureManager.GetTexture("ammo_bombs").BaseTexture as Texture2D;
                else
                {
                    bombBackground = TextureManager.GetTexture("ammo_bombs_low").BaseTexture as Texture2D;
                    bombCountColor = Color.Red;
                }


                if (MGPercentage() > 25f) MGBackground = TextureManager.GetTexture("ammo_mg").BaseTexture as Texture2D;
                else
                {
                    MGBackground = TextureManager.GetTexture("ammo_mg_low").BaseTexture as Texture2D;
                    mgCountColor = Color.Red;
                }

                float scale = 0.40f;

                ScreenManager.SpriteBatch.Begin();

                ScreenManager.SpriteBatch.Draw(MGBackground, mgPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.Draw(bombBackground, bombPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                ScreenManager.SpriteBatch.Draw(MGBackground, mgPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.Draw(bombBackground, bombPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                if (mgRefillGreen.IsRunning)
                    ScreenManager.SpriteBatch.Draw(greenBackground, mgPos, null, Color.White * (1 - (((float)mgRefillGreen.ElapsedMilliseconds) / 3000f)), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                if (bRefillGreen.IsRunning)
                    ScreenManager.SpriteBatch.Draw(greenBackground, bombPos, null, Color.White * (1 - (((float)bRefillGreen.ElapsedMilliseconds) / 3000f)), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                Text.DrawText(ScreenManager.Cartoon14regular, ProjectileWeapon.AmmoCount.ToString(), mgCountColor, mgPos + new Vector2(40, 23), 1f);
                Text.DrawText(ScreenManager.Cartoon14regular, BombWeapon.AmmoCount.ToString(), bombCountColor, bombPos + new Vector2(40, 23), 1f);

                ScreenManager.SpriteBatch.End();
            }
        }
    }
}
