using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Interfaces;
using PoorEngine.Helpers;
using PoorEngine.Managers;

namespace PoorEngine.SceneObject
{
    public class CannonProjectile : Projectile
    {
        public CannonProjectile(Vector2 pos, Vector2 velocity, float orientation, IPoorWeaponHolder origin) :
            base("bullet", pos, velocity, origin)
        {
            _orientation = orientation;
            float xFactor = (float)Math.Sin(CalcHelper.DegreeToRadian(_orientation));
            float yFactor = (float)-Math.Cos(CalcHelper.DegreeToRadian(_orientation));
            Vector2 boostFactor = new Vector2(xFactor*3, yFactor*3);

            _velocity = velocity + boostFactor;
            _invulnerableTime = 0.8f;
            Damage = 50;
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            base.Collide(collidingWith);
            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Airplane)))
            {
                ParticleManager.ProjectileHit.AddParticles(new Vector2(Position.X - 10, Position.Y));
                SoundFxLibrary.GetFx("hitplane1").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.05f),
                                    CalcHelper.RandomBetween(-0.5f, 0.1f), CalcHelper.CalcPan(Position).X * 1.8f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Position.Y > GameHelper.GroundLevel + 10)
            {
                SoundFxLibrary.GetFx("hitplane1").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.05f), CalcHelper.RandomBetween(-1.0f, -0.4f), CalcHelper.CalcPan(Position).X * 1.5f);
                SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position - new Vector2(0, 15), 0f, new Vector2(0.2f, 0.2f), 200, 50, false, 0.9f));

                SceneGraphManager.RemoveObject(this);
            }

        }
    }
}
