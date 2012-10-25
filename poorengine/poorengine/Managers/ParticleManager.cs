#region File Description
//-----------------------------------------------------------------------------
// ParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Helpers;
using PoorEngine.Textures;
using PoorEngine.Particles;
#endregion

namespace PoorEngine.Managers
{
    public class ParticleManager : GameComponent
    {

        private static ExplosionParticleSystem _explosionParticles = null;
        public static ExplosionParticleSystem Explosion { get { return _explosionParticles; } }

        private static GroundExplosion _groundExplosion = null;
        public static GroundExplosion GroundExplosion { get { return _groundExplosion; } }

        private static ProjectileHit _projectileHit = null;
        public static ProjectileHit ProjectileHit { get { return _projectileHit; } }

        private static ShrapnelExplosion _shrapnelExplosion = null;
        public static ShrapnelExplosion ShrapnelExplosion { get { return _shrapnelExplosion; } }

        private static BlackSmoke _blackSmoke = null;
        public static BlackSmoke BlackSmoke { get { return _blackSmoke; } }

        private static WhiteSmoke _whiteSmoke = null;
        public static WhiteSmoke WhiteSmoke { get { return _whiteSmoke; } }

        private static GroundDust _groundDust = null;
        public static GroundDust GroundDust { get { return _groundDust; } }

        private static MuzzleFlash _muzzleFlash = null;
        public static MuzzleFlash MuzzleFlash { get { return _muzzleFlash; } }

        private static Fire _fire = null;
        public static Fire Fire { get { return _fire; } }

        private static List<ParticleSystem> _systems = null;

        public ParticleManager(Game game)
            :base(game)
        {
            _systems = new List<ParticleSystem>();
        }

        public override void Initialize()
        {
            base.Initialize();
            _explosionParticles = new ExplosionParticleSystem(EngineManager.Game, 3);
            _groundExplosion = new GroundExplosion(EngineManager.Game, 8);
            _projectileHit = new ProjectileHit(EngineManager.Game, 5);
            _shrapnelExplosion = new ShrapnelExplosion(EngineManager.Game, 3);
            _blackSmoke = new BlackSmoke(EngineManager.Game, 3);
            _whiteSmoke = new WhiteSmoke(EngineManager.Game, 3);
            _groundDust = new GroundDust(EngineManager.Game, 3);
            _muzzleFlash = new MuzzleFlash(EngineManager.Game, 3);
            _fire = new Fire(EngineManager.Game, 3);
            _explosionParticles.Initialize();
            _groundExplosion.Initialize();
            _projectileHit.Initialize();
            _shrapnelExplosion.Initialize();
            _blackSmoke.Initialize();
            _whiteSmoke.Initialize();
            _groundDust.Initialize();
            _muzzleFlash.Initialize();
            _fire.Initialize();
        }

        public static void LoadContent()
        {
            _systems.Add(_explosionParticles);
            _systems.Add(_groundExplosion);
            _systems.Add(_projectileHit);
            _systems.Add(_shrapnelExplosion);
            _systems.Add(_blackSmoke);
            _systems.Add(_whiteSmoke);
            _systems.Add(_groundDust);
            _systems.Add(_muzzleFlash);
            _systems.Add(_fire);
        }

        public static void UnloadContent()
        {
            for (int i=0; i< _systems.Count; i++)
            {
                _systems[i] = null;
            }
            _systems.Clear();
        }

        public static void Draw(GameTime gameTime)
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                _systems[i].Draw(gameTime);
            }
        }

        public static new void Update(GameTime gameTime)
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                _systems[i].Update(gameTime);
            }
        }
    }
}
