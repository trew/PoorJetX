using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;

namespace PoorEngine.SceneObject
{
    public class GroundBattleVehicle : GroundVehicle, IPoorWeaponHolder
    {
        private float _weaponOrientation;
        public float Orientation { get { return _weaponOrientation; } }

        private string _textureNameWeapon;

        public GroundBattleVehicle(int maxHealth, string textureName) // Add weapon parameter (obj/str)
            : base(maxHealth, textureName + "_body", textureName + "_destroyed")
        {
            Scale = new Vector2(0.2f, 0.2f);
            _textureNameWeapon = textureName + "_weapon";
        }

        public override void Update(GameTime gameTime)
        {
            if (!_destroyed) UpdateAI(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw _textureNameWeapon in the right orientation
            base.Draw(gameTime);
        }

        private void UpdateAI(GameTime gametime)
        {
            // Aim and fire and shit?
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            TextureManager.RemoveTexture(_textureNameWeapon);
        }
    }
}
