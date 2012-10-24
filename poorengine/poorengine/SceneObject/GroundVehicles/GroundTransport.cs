using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.SceneObject
{
    public class GroundTransport : GroundVehicle, IPoorEnemy
    {
        private bool _requiredForVictory;
        public bool RequiredForVictory { get { return _requiredForVictory; } }

        private string RndEnemyTex()
        {
            return "enemy_transport" + (int)CalcHelper.RandomBetween(1f, 3.99f);
        }

        public GroundTransport(int maxHealth, bool requiredForVictory)
            :base(maxHealth, "", "", "civilian")
        {
            _requiredForVictory = requiredForVictory;
            TextureName = RndEnemyTex();
            TextureNameDestroyed = TextureName + "_destroyed";

            Velocity = new Vector2(CalcHelper.RandomBetween(1, 3.5f), 0);
            Scale = new Vector2(1f, 1f);
        }

        public GroundTransport(int maxHealth, bool requiredForVictory, bool boss)
            : base(maxHealth, "", "", "burgerboss")
        {
            _requiredForVictory = requiredForVictory;
            TextureName = "enemy_burgerboss_body";
            TextureNameDestroyed = "enemy_burgerboss_destroyed";

            Velocity = new Vector2(3.5f, 0);
            Scale = new Vector2(1f, 1f);
        }

        public override void GetPoints()
        {
            if (_type.Equals("civilian"))
                EngineManager.Score += 1;
            else if (_type.Equals("burgerboss"))
                EngineManager.Score += 30;

        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed && _health <= 0)
            {
                EngineManager.Score += 1;
            }
            base.Update(gameTime);

            if (!RequiredForVictory)
            {
                if (!_destroyed && Position.X < CameraManager.Camera.Pos.X - 2000)
                {
                    SoundFxManager.RemoveFx(_engineFX_id);
                    SoundFxManager.RemoveFx(_fireBulletFX_id);
                    SceneGraphManager.RemoveObject(this);
                    return;
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (SceneGraphManager.TimeOfDay < MathHelper.Pi/2f && _type.Equals("civilian"))
            {
                Texture2D headlights = TextureManager.GetTexture("enemy_transport_headlights").BaseTexture as Texture2D;

                Vector2 normalizedPosition = CameraManager.Camera.Normalize(Position + new Vector2(107, 15));

                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(headlights, new Rectangle((int)normalizedPosition.X, (int)normalizedPosition.Y, 116, 33), Color.White);
                ScreenManager.SpriteBatch.End();

            }


            if (RequiredForVictory && !IsDead)
                DrawArrow("arrow", 0.4f, true);
        }

    }
}
