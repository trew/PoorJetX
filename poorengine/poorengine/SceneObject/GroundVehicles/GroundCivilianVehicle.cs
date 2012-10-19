using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.SceneObject
{
    public class GroundCivilianVehicle : GroundVehicle
    {
        public GroundCivilianVehicle(int maxHealth, string textureName)
            :base(maxHealth, textureName + "_body", textureName + "_destroyed")
        {
            Scale = new Vector2(0.4f, 0.4f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
