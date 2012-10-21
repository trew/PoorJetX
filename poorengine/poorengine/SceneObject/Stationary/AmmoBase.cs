using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Helpers;
using System.Diagnostics;

namespace PoorEngine.SceneObject
{
    public class AmmoBase : PoorSceneObject, IPoorLoadable, IPoorDrawable, IPoorUpdateable
    {
        private Stopwatch bRefillTimer;
        private Stopwatch mgRefillTimer;


        public AmmoBase()
            :base("ammobase")
        {
            Z = 0.99991f;

            bRefillTimer = new Stopwatch();
            bRefillTimer.Start();

            mgRefillTimer = new Stopwatch();
            mgRefillTimer.Start();
        }


        public void Update(GameTime gameTime)
        {

            if(CalcHelper.DistanceBetween(EngineManager.Player.Position, this.Position) < 300f)
            {
                if (bRefillTimer.ElapsedMilliseconds > 1000)
                {
                    EngineManager.Player.RefillBombs(1);
                    bRefillTimer.Restart();
                }

                if (mgRefillTimer.ElapsedMilliseconds > 30)
                {
                    EngineManager.Player.RefillMG(1);
                    
                    mgRefillTimer.Restart();
                }
                
            }
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(
                texture,
                CameraManager.Camera.Normalize(Position),
                null,
                Color.White,
                0f,
                new Vector2(0, texture.Height),
                1f,
                SpriteEffects.None,
                0f);

            ScreenManager.SpriteBatch.End();
        }

        public void LoadContent()
        {
            
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/" + TextureName), TextureName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }

    }
}
