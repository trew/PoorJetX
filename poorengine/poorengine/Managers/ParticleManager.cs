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

        private static AirplaneExplosion _airplaneExplosion = null;
        public static AirplaneExplosion AirplaneExplosion { get { return _airplaneExplosion; } }

        private static ShrapnelExplosion _shrapnelExplosion = null;
        public static ShrapnelExplosion ShrapnelExplosion { get { return _shrapnelExplosion; } }

        private static List<ParticleSystem> _systems = null;

        public ParticleManager(Game game)
            :base(game)
        {
            _systems = new List<ParticleSystem>();
        }

        public override void Initialize()
        {
            base.Initialize();
            _explosionParticles = new ExplosionParticleSystem(EngineManager.Game, 1);
            _groundExplosion = new GroundExplosion(EngineManager.Game, 1);
            _projectileHit = new ProjectileHit(EngineManager.Game, 1);
            _airplaneExplosion = new AirplaneExplosion(EngineManager.Game, 1);
            _shrapnelExplosion = new ShrapnelExplosion(EngineManager.Game, 1);
            _explosionParticles.Initialize();
            _groundExplosion.Initialize();
            _projectileHit.Initialize();
            _airplaneExplosion.Initialize();
            _shrapnelExplosion.Initialize();
        }

        public static void LoadContent()
        {
            _systems.Add(_explosionParticles);
            _systems.Add(_groundExplosion);
            _systems.Add(_projectileHit);
            _systems.Add(_airplaneExplosion);
            _systems.Add(_shrapnelExplosion);
        }

        public static void UnloadContent()
        {
            _systems.Clear();
        }

        public static new void Draw(GameTime gameTime)
        {
            foreach (ParticleSystem syst in _systems)
            {
                syst.Draw(gameTime);
            }
        }

        public static new void Update(GameTime gameTime)
        {
            foreach (ParticleSystem syst in _systems)
            {
                syst.Update(gameTime);
            }
        }
    }
}
