using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Helpers;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;

namespace PoorEngine.SceneObject
{
    public class BombProjectile : Projectile
    {
        private double _spread;
        protected int _soundFX_id;
        public BombProjectile(Vector2 pos, Vector2 velocity, IPoorWeaponHolder origin) :
            base("bomb2", pos, velocity, origin)
        {
            Scale = new Vector2(0.13f, 0.13f);
            _invulnerableTime = 1.0f;
            _soundFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("bombwhistle"));
            SoundFxManager.GetByID(_soundFX_id).IsLooped = true;
            Damage = 10000;

        }
        

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Lets whistle!
            if (_velocity.Y > 4)
            {
                if (SoundFxManager.GetByID(_soundFX_id).State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                    SoundFxManager.GetByID(_soundFX_id).Play();

                SoundFxManager.GetByID(_soundFX_id).Pan = CalcHelper.CalcPan(Position).X * 1.7f;

                float pitch = 1f - ((_velocity.Y - 4f) / 10f); // Epic formula. Great taste. Crunchy on the outside. Chewy in the middle!
                SoundFxManager.GetByID(_soundFX_id).Pitch = MathHelper.Clamp(pitch - 0.3f, -1, 1);
                SoundFxManager.GetByID(_soundFX_id).Volume = SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * MathHelper.Clamp(0.85f - pitch, 0f, 1f) * 0.3f);
            }

            if (Position.Y > GameHelper.GroundLevel + 10)
            {

                ParticleManager.GroundExplosion.AddParticles(Position,  _velocity.X * 8f, 0);
                SoundFxLibrary.GetFx("bomb2").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.35f),
                                           CalcHelper.RandomBetween(-0.5f, 1f), CalcHelper.CalcPan(Position).X * 1.2f);

                // Remove whisteling sound
                SoundFxManager.GetByID(_soundFX_id).Stop();
                SoundFxManager.RemoveFx(_soundFX_id);

                SceneGraphManager.RemoveObject(this);
            }

        }
        public override void Collide(PoorSceneObject collidingWith)
        {
            base.Collide(collidingWith);
            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Airplane)))
            {
                ParticleManager.ShrapnelExplosion.AddParticles(new Vector2(Position.X - 10, Position.Y), 0f, 360f);
                ParticleManager.Explosion.AddParticles(Position);
                SoundFxLibrary.GetFx("bomb4").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.7f),
                                    CalcHelper.RandomBetween(-0.5f, 0.2f), CalcHelper.CalcPan(Position).X * 1.8f);
            }
            SoundFxManager.RemoveFx(_soundFX_id);
            ParticleManager.GroundExplosion.AddParticles(Position, 0, 35);
        }
    }
}
