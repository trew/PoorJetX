using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace PoorEngine.SceneObject
{
    public abstract class Weapon: PoorSceneObject, IPoorUpdateable, IPoorLoadable 
    {
        protected IPoorWeaponHolder _owner;
        public IPoorWeaponHolder Owner { get { return _owner; } }

        protected Stopwatch _reloadTimer;
        public Stopwatch ReloadTimer { get { return _reloadTimer; } }

        public Weapon(IPoorWeaponHolder owner) :
            base("")
        {
            _owner = owner;
            _reloadTimer = new Stopwatch();
            _reloadTimer.Start();
        }

        public abstract bool Fire();

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }
    }
}
