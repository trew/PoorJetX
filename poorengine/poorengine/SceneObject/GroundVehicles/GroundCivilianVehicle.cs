using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;

namespace PoorEngine.SceneObject
{
    public class GroundCivilianVehicle : GroundVehicle
    {
        public GroundCivilianVehicle(int maxHealth, string textureName)
            :base(maxHealth, textureName + "_body", textureName + "_destroyed")
        {
            _type = "civilian";
            Scale = new Vector2(0.4f, 0.4f);
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
