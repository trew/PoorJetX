using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class Cannon : Weapon
    {
        const int BULLETSPERMINUTE = 10;

        public float Angle { get; set; }

        public Cannon(IPoorWeaponHolder owner)
            :base(owner)
        {
        }

        public override bool Fire()
        {
            if (!_reloadTimer.IsRunning) { _reloadTimer.Restart(); }
            if (_reloadTimer.ElapsedMilliseconds > 60000 / BULLETSPERMINUTE)
            {

                SoundFxLibrary.GetFx("firebullet").Play(
                                                        SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Owner.Position) * 0.1f),
                                                        CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                        CalcHelper.CalcPan(Owner.Position).X);

                SceneGraphManager.AddObject(new CannonProjectile(Owner.Position,
                                                                 Owner.Velocity, Angle, Owner));
                _reloadTimer.Restart();
                return true;
            }
            return false;
        }
    }
}
