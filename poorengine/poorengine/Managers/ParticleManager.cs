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

        public ParticleManager(Game game)
            :base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            _explosionParticles = new ExplosionParticleSystem(EngineManager.Game, 1);
            EngineManager.Game.Components.Add(_explosionParticles);

            _groundExplosion = new GroundExplosion(EngineManager.Game, 1);
            EngineManager.Game.Components.Add(_groundExplosion);

            _projectileHit = new ProjectileHit(EngineManager.Game, 1);
            EngineManager.Game.Components.Add(_projectileHit);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
