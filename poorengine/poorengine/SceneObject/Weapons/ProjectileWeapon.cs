using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;
using PoorEngine.Particles;

namespace PoorEngine.SceneObject
{
    public class ProjectileWeapon : Weapon
    {
        public const int BULLETSPERSECOND = 30;
        public const int MAXBULLETS = 200;
        public int MAX_BULLETS { get { return MAXBULLETS; } }


        protected int _bulletCount = MAXBULLETS;
        public int AmmoCount { get { return _bulletCount; } }

        public bool Refilled;
        MuzzleFlashSmall _muzzleFlash;

        public ProjectileWeapon(IPoorWeaponHolder owner)
            :base(owner)
        {
            _muzzleFlash = new MuzzleFlashSmall(EngineManager.Game, 4);
            EngineManager.Game.Components.Add(_muzzleFlash);
        }

        public override void Reset()
        {
            base.Reset();
            _bulletCount = MAXBULLETS;
        }

        public override bool Fire()
        {
            if (_bulletCount <= 0) return false;
            if (!_reloadTimer.IsRunning) { _reloadTimer.Restart(); }
            if (_reloadTimer.ElapsedMilliseconds > 1000 / BULLETSPERSECOND)
            {

                SoundFxLibrary.GetFx("firebullet").Play(
                                                        SoundFxManager.GetVolume("Sound", 0.1f),
                                                        CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                        CalcHelper.CalcPan(Position).X);

                SceneGraphManager.AddObject(new BulletProjectile(CalcHelper.calculatePoint(Owner.Position, Owner.Orientation - 10, 30f),
                                                                 Owner.Velocity, 15f, Owner.Orientation, 3f, Owner));

                _muzzleFlash.AddParticles(CalcHelper.calculatePoint(Owner.Position + Position, Owner.Orientation - 5f, 30f), Owner.Orientation, 0f);

                _reloadTimer.Restart();
                _bulletCount--;
                return true;
            }
            return false;
        }

        public void Refill(int amount) 
        {
            int oldCount = _bulletCount;
            _bulletCount = CalcHelper.Clamp(_bulletCount + amount, 0, MAXBULLETS);

            if (oldCount != _bulletCount)
                Refilled = true;
        }

        public override void Update(GameTime gameTime)
        {
            //Do heating logic here
        }
    }
}
