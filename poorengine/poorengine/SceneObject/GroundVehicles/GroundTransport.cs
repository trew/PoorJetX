using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;

namespace PoorEngine.SceneObject
{
    public class GroundTransport : GroundVehicle, IPoorEnemy
    {
        private string RndEnemyTex()
        {
            return "enemy_transport" + (int)CalcHelper.RandomBetween(1f, 3.99f);
        }

        public GroundTransport(int maxHealth)
            :base(maxHealth, "", "")
        {
            TextureName = RndEnemyTex();
            TextureNameDestroyed = TextureName + "_destroyed";

            Velocity = new Vector2(CalcHelper.RandomBetween(1, 3.5f), 0);

            _type = "civilian";
            Scale = new Vector2(1f, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed && _health <= 0)
            {
                EngineManager.Score += 1;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
