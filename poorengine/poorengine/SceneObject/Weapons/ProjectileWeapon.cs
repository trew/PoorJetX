﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;

namespace PoorEngine.SceneObject
{
    public class ProjectileWeapon : Weapon
    {
        public const int BULLETSPERSECOND = 30;
        public const int MAXBULLETS = 200;

        protected int _bulletCount = MAXBULLETS;
        public int BulletCount { get { return _bulletCount; } }

        public ProjectileWeapon(IPoorWeaponHolder owner)
            :base(owner)
        {
        }

        public override bool Fire()
        {
            if (_bulletCount <= 0) return false;
            if (!_reloadTimer.IsRunning) { _reloadTimer.Restart(); }
            if (_reloadTimer.ElapsedMilliseconds > 1000 / BULLETSPERSECOND)
            {

                SoundFxLibrary.GetFx("firebullet").Play(
                                                        0.1f,
                                                        CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                        CalcHelper.CalcPan(Position).X);

                SceneGraphManager.AddObject(new BulletProjectile(CalcHelper.calculatePoint(Owner.Position, Owner.Orientation, 30f),
                                                                 Owner.Velocity, 15f, Owner.Orientation, 3f, Owner));
                _reloadTimer.Restart();
                _bulletCount--;
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            //Do heating logic here
        }
    }
}