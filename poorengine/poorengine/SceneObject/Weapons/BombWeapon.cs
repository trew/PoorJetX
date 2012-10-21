using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;

namespace PoorEngine.SceneObject
{
    public class BombWeapon : Weapon
    {
        public const int MAXBOMBS = 15;
        public const float BOMBSPERSECOND = 1.5f;

        protected int _bombCount = MAXBOMBS;
        public int AmmoCount { get { return _bombCount; } }
        public int MAX_BOMBS { get { return MAXBOMBS; } }

        public bool Refilled;

        public BombWeapon(IPoorWeaponHolder owner)
            : base(owner)
        {
        }

        public override bool Fire()
        {
            if (_bombCount <= 0) return false;
            if (!_reloadTimer.IsRunning) { _reloadTimer.Restart(); }
            if (_reloadTimer.ElapsedMilliseconds > 1000 / BOMBSPERSECOND)
            {

                SoundFxLibrary.GetFx("bombdrop").Play(SoundFxManager.GetVolume("Sound", 0.3f), CalcHelper.RandomBetween(0.8f, 0.2f), CalcHelper.CalcPan(Owner.Position).X * 1.8f);
                SceneGraphManager.AddObject(new BombProjectile(CalcHelper.calculatePoint(Owner.Position, Owner.Orientation + 90, 10f), Owner.Velocity, Owner));
                _bombCount--;
                _reloadTimer.Restart();
                return true;
            }
            return false;
        }

        public void Refill(int amount)
        {
            int oldCount = _bombCount;
            _bombCount = CalcHelper.Clamp(_bombCount + amount, 0, MAXBOMBS);

            if(oldCount != _bombCount)
                Refilled = true;
        }
    }
}
