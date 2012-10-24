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
            Scale = new Vector2(1, 1);

            bRefillTimer = new Stopwatch();
            bRefillTimer.Start();

            mgRefillTimer = new Stopwatch();
            mgRefillTimer.Start();
        }


        public void Update(GameTime gameTime)
        {

            if(CalcHelper.DistanceBetween(EngineManager.Player.Position, this.Position) < 300f)
            {
                if (bRefillTimer.ElapsedMilliseconds > 700)
                {
                    EngineManager.Player.RefillBombs(1);
                    bRefillTimer.Restart();
                }

                if (mgRefillTimer.ElapsedMilliseconds > 5)
                {
                    EngineManager.Player.RefillMG(2);
                    
                    mgRefillTimer.Restart();
                }
                
            }

            if (Position.X < CameraManager.Camera.Pos.X - 2000)
            {
                SceneGraphManager.RemoveObject(this);
                return;
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
                SceneGraphManager.TODcolor,
                0f,
                new Vector2(0, texture.Height),
                1f,
                SpriteEffects.None,
                0f);

            ScreenManager.SpriteBatch.End();

            if (GameHelper.IsOutOfView(Position + new Vector2(0, -30)) != 0)
                DrawArrow("ammobase_icon", 1.0f, false);

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
