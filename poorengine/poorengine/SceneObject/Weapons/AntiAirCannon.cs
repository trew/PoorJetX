using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using System.Diagnostics;
using PoorEngine.Particles;
using Microsoft.Xna.Framework;

namespace PoorEngine.SceneObject
{
    public class AntiAirCannon : Weapon
    {
        const int BULLETSPERMINUTE = 1400;
        const int BULLETSPERBURST = 4;
        const int BURSTDELAY = 1500;

        bool firstBurst;
        private int bulletsThisBurst;
        private Stopwatch _burstTimer;
        ProjectileHit _muzzleFlash;

        Vector2[] gunOffset;

        public float Angle { get; set; }

        public AntiAirCannon(IPoorWeaponHolder owner)
            :base(owner)
        {
            firstBurst = true;
            bulletsThisBurst = 0;
            _burstTimer = new Stopwatch();
            _burstTimer.Start();

            _muzzleFlash = new ProjectileHit(EngineManager.Game, 7);
            EngineManager.Game.Components.Add(_muzzleFlash);

            gunOffset = new Vector2[4];
            gunOffset[0] = new Vector2(0, -3);
            gunOffset[1] = new Vector2(-3, 0);
            gunOffset[2] = new Vector2(-6, 3);
            gunOffset[3] = new Vector2(3, -6);           
        }

        public override bool Fire()
        {
            if (!_reloadTimer.IsRunning) { _reloadTimer.Restart(); }
            if (!_burstTimer.IsRunning) { _burstTimer.Restart(); }

            if (_burstTimer.ElapsedMilliseconds > BURSTDELAY && bulletsThisBurst < BULLETSPERBURST || firstBurst == true)
            {
                if (_reloadTimer.ElapsedMilliseconds > 60000 / BULLETSPERMINUTE)
                {
                    SoundFxLibrary.GetFx("firebullet").Play(
                                                            SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Owner.Position) * 0.1f),
                                                            CalcHelper.RandomBetween(-0.6f, -0.3f),
                                                            CalcHelper.CalcPan(Owner.Position).X);

                    SceneGraphManager.AddObject(new AAProjectile(
                                                                     CalcHelper.calculatePoint(Owner.Position + Position, Angle, 45f),
                                                                     Owner.Velocity, 5f, Angle, 2f, Owner));

                    _muzzleFlash.AddParticles(CalcHelper.calculatePoint(Owner.Position + Position + gunOffset[bulletsThisBurst % 4], Angle, 45f), Angle, 0);

                    _reloadTimer.Restart();
                    bulletsThisBurst++;

                    if (bulletsThisBurst >= BULLETSPERBURST)
                    {
                        _burstTimer.Restart();
                        bulletsThisBurst = 0;

                        if(firstBurst) firstBurst = false;
                    }
                }
            }
            return false;
        }
    }
}
