﻿#region File Description
//-----------------------------------------------------------------------------
// ExplosionParticleSystem.cs
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
#endregion

namespace PoorEngine.Particles
{
    /// <summary>
    /// ExplosionParticleSystem is a specialization of ParticleSystem which creates a
    /// fiery explosion. It should be combined with ExplosionSmokeParticleSystem for
    /// best effect.
    /// </summary>
    public class ShrapnelExplosion : ParticleSystem
    {
        public ShrapnelExplosion(Game game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        /// <summary>
        /// Set up the constants that will give this particle system its behavior and
        /// properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Textures/bomb";

            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 10f;
            maxInitialSpeed = 300f;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            // explosions should be relatively short lived
            minLifetime = .2f;
            maxLifetime = 1.5f;

            minScale = 0.2f;
            maxScale = 0.6f;

            minNumParticles = 30;
            maxNumParticles = 70;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            blendState = BlendState.AlphaBlend;

            DrawOrder = AdditiveDrawOrder;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);

            // The base works fine except for acceleration. Explosions move outwards,
            // then slow down and stop because of air resistance. Let's change
            // acceleration so that when the particle is at max lifetime, the _velocity
            // will be zero.

            // We'll use the equation vt = v0 + (a0 * t). (If you're not familar with
            // this, it's one of the basic kinematics equations for constant
            // acceleration, and basically says:
            // _velocity at time t = initial _velocity + acceleration * t)
            // We'll solve the equation for a0, using t = p.Lifetime and vt = 0.
            //float YAcc = Math.Abs(p.Velocity.Y) / p.Lifetime;
            p.Acceleration = -p.Velocity / p.Lifetime; // new Vector2(-p.Velocity.X / p.Lifetime, YAcc);
        }
    }
}