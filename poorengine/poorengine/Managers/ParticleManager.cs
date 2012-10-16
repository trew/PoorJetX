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
    public class ParticleManager : DrawableGameComponent
    {

        private static ExplosionParticleSystem _explosionParticles = null;
        public static ExplosionParticleSystem Explosion { get { return _explosionParticles; } }

        private static GroundExplosion _groundExplosion = null;
        public static GroundExplosion GroundExplosion { get { return _groundExplosion; } }

        private static ProjectileHit _projectileHit = null;
        public static ProjectileHit ProjectileHit { get { return _projectileHit; } }

        private static List<ParticleSystem> _systems = null;

        public ParticleManager(Game game)
            :base(game)
        {
            _systems = new List<ParticleSystem>();
        }

        public override void Initialize()
        {
            base.Initialize();
            _explosionParticles.Initialize();
            _groundExplosion.Initialize();
            _projectileHit.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _explosionParticles = new ExplosionParticleSystem(EngineManager.Game, 1);
            _groundExplosion = new GroundExplosion(EngineManager.Game, 1);
            _projectileHit = new ProjectileHit(EngineManager.Game, 1);
            _systems.Add(_explosionParticles);
            _systems.Add(_groundExplosion);
            _systems.Add(_projectileHit);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _systems.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach (ParticleSystem syst in _systems)
            {
                syst.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (ParticleSystem syst in _systems)
            {
                syst.Update(gameTime);
            }
        }
    }
}
