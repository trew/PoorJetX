using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Managers;
using PoorEngine.Interfaces;
using PoorEngine.Helpers;
using Microsoft.Xna.Framework;

namespace PoorEngine.SceneObject
{
    public class BulletProjectile: Projectile
    {
        private double _spread;
        public BulletProjectile(Vector2 pos, Vector2 velocity, float velocityBoost, float orientation, float spreadDegrees, IPoorWeaponHolder origin) :
            base("bullet", pos, velocity, origin)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            _spread = (rnd.NextDouble() * spreadDegrees) / 2.0;

            if (rnd.NextDouble() > 0.5)
            {
                _spread = -_spread;
            }
            _orientation = orientation + (float)_spread;
            float xFactor = (float)Math.Sin(CalcHelper.DegreeToRadian(_orientation));
            float yFactor = (float)-Math.Cos(CalcHelper.DegreeToRadian(_orientation));
            Vector2 boostFactor = new Vector2(xFactor * velocityBoost, yFactor * velocityBoost);

            _velocity = velocity + boostFactor;
            _invulnerableTime = 0.5f;
            Damage = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Position.Y > EngineManager.Device.Viewport.Height)
            {
                SoundFxLibrary.GetFx("hitplane1").Play(CalcHelper.CalcVolume(Position) * 0.05f, CalcHelper.RandomBetween(-1.0f, -0.4f), CalcHelper.CalcPan(Position).X * 1.5f);
                SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position - new Vector2(0, 15), 0f, new Vector2(0.2f, 0.2f), 200, 50, false, 0.9f));

                SceneGraphManager.RemoveObject(this);
            }

        }
    }
}
