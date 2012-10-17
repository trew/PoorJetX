using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.GameComponents;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class PlayerAirplane : Airplane
    {
        public PlayerAirplane():
            base(2000, "apTex1")
        {
        }

        public override void HandleDebugInput(Input input)
        {
            if (input.IsNewKeyPress(Keys.Y))
            {
                TakeDamage(200);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                _orientation -= 4;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                _thrust = 0;
            }

            if (input.IsNewKeyPress(Keys.O))
            {
                ParticleManager.Explosion.AddParticles(CameraManager.Camera.Pos + new Vector2(EngineManager.Device.Viewport.Width / 2 + 100,EngineManager.Device.Viewport.Height -30 ));
            }
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (IsCrashing) return;

            if (collidingWith.GetType() == typeof(Projectile))
            {
                Projectile proj = (Projectile)collidingWith;
                TakeDamage(proj.Damage);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsDead)
                CameraManager.Camera.Stop();

        }

        protected override void UpdatePhysics(GameTime gameTime)
        {
            base.UpdatePhysics(gameTime);
            // Standardize angle-values
            _orientation = CalcHelper.formatAngle(_orientation);
            _velocityAngle = CalcHelper.formatAngle(_velocityAngle);

            // Calculate alpha-diff
            double velocityAngleRotationSpeed = Math.Max(_airSpeed, _gravity) / (_weight * 3);
            double diff = _orientation - _velocityAngle + 180;
            double posDiff = Math.Abs(diff - 180);


            // Adjust _velocityAngle towards the airplane _orientation 
            if (posDiff < velocityAngleRotationSpeed)
            {
                _velocityAngle = _orientation;
            }
            else if (diff >= 0 && diff < 180 || diff > 359)
            {
                _velocityAngle -= velocityAngleRotationSpeed;
            }
            else if (diff >= 180 || diff < 0)
            {
                _velocityAngle += velocityAngleRotationSpeed;
            }

            // Calculate angle of attack (alpha)
            _angleOfAttack = _velocityAngle - _orientation;
            if (_angleOfAttack < -180)
            {
                _angleOfAttack = _angleOfAttack + 360;
            }
            else if (_angleOfAttack > 180)
            {
                _angleOfAttack = (360 - _angleOfAttack) * -1;
            }

            // Calculate movement-direction (X/Y-movement-ratio)
            double newX = Math.Sin(CalcHelper.DegreeToRadian(_velocityAngle));
            double newY = -Math.Cos(CalcHelper.DegreeToRadian(_velocityAngle));

            // Calculate _lift and drag
            _lift = (double)Math.Min(3, Math.Pow(Math.Sqrt(Math.Abs(_velocity.X)), 1.5));
            _lift = Math.Max(_lift, 0.8);

            double drag = Math.Sqrt(Math.Abs(_angleOfAttack) / 90);

            // Change rotationspeed of the airplane depending on Angle of Attack
            _angleSpeedModifier = Math.Abs(180 - _velocityAngle);
            _angleSpeedModifier = Math.Sqrt(_angleSpeedModifier / 90);

            _angleSpeedModifier = 1;

            // Adds 'acceleration'
            if (_airSpeed < _thrust)
            {
                _airSpeed += 0.025 / _angleSpeedModifier;
            }
            else if (_airSpeed > _thrust)
            {
                _airSpeed -= 0.010 / _angleSpeedModifier;
            }

            double movementMultiplier = _airSpeed - drag;

            float xmod = (float)(newX * movementMultiplier);
            float ymod = (float)(((newY * movementMultiplier)) + _gravity - _lift);
            ymod += (float)(9.8 * gameTime.ElapsedGameTime.TotalSeconds);

            
            // Save old pos, used for speedcalculations
            _oldPos = Position;
            Position += new Vector2(xmod, ymod);
            _velocity = Position - _oldPos;

            _linearVelocity = Math.Sqrt(
                Math.Pow((Math.Max(Position.X, _oldPos.X) - Math.Min(Position.X, _oldPos.X)), 2) +
                Math.Pow((Math.Max(Position.Y, _oldPos.Y) - Math.Min(Position.Y, _oldPos.Y)), 2));

            if (_airSpeed + 0.1 < _linearVelocity)
            {
                _airSpeed += 0.025 / _angleSpeedModifier;
            }
            /*
            else if (_airSpeed > _linearVelocity)
            {
                _airSpeed -= 0.015 / _angleSpeedModifier;
            }
            */
            if (!IsCrashing)
                _airSpeed = Math.Min(_airSpeed, 8);
            else
                _airSpeed = Math.Min(_airSpeed, 20);
        }

        public override void HandleInput(Input input)
        {
            HandleDebugInput(input);

            if (IsCrashing) return;

            double forceIncreaseAmount = MathHelper.Clamp((float)(0.1 / _linearVelocity), 0.002f, 0.4f);
            double maxThrust = 7;
            double maxForce = MathHelper.Clamp((float)(3 / _linearVelocity), 0.3f, 1.3f);
            double forceResetAmount = 0.085;


            if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                forceIncreaseAmount *= 3;
                maxForce = MathHelper.Clamp((float)(7.5 / _linearVelocity), 1.2f, 1.5f);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_airSpeed > 7.2 && _lforce > 0.2)
                {
                    drawWingtipVortices();
                }

                _orientation -= _lforce;
                _lforce = MathHelper.Clamp((float)(_lforce + forceIncreaseAmount), 0f, (float)maxForce);
                
            }
            else
            {
                _lforce = MathHelper.Clamp((float)(_lforce -= forceResetAmount), 0f, 10f);
                _orientation -= _lforce;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (_airSpeed > 7.2 && _rforce > 0.2)
                {
                    drawWingtipVortices();
                }

                _orientation += _rforce;
                _rforce = MathHelper.Clamp((float)(_rforce + forceIncreaseAmount), 0f, (float)maxForce);
            }
            else
            {
                _rforce = MathHelper.Clamp((float)(_rforce -= forceResetAmount), 0f, 10f);
                _orientation += _rforce;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
            {
                if (_thrust < maxThrust)
                    _thrust += 0.05;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
            {
                if (_thrust >= 0.05)
                    _thrust -= 0.05;
            }

            if (input.IsNewKeyPress(Keys.Space))
            {
                if (AmmoManager.dropBomb())
                {
                    SoundFxLibrary.GetFx("bombdrop").Play(0.3f, CalcHelper.RandomBetween(0.8f, 0.2f), CalcHelper.CalcPan(Position).X * 1.8f);
                    SceneGraphManager.AddObject(new Projectile(CalcHelper.calculatePoint(Position, Orientation + 90, 10f), Velocity, "bomb2", 0.13f, this));
                }
            }

            if (input.LastKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (AmmoManager.fireBullet())
                {

                    SoundFxLibrary.GetFx("firebullet").Play(
                                                            0.1f,
                                                            CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                            CalcHelper.CalcPan(Position).X );

                    SceneGraphManager.AddObject(new Projectile(CalcHelper.calculatePoint(Position, Orientation, 30f), Velocity, 15f, Orientation, 3f, "bullet", this));
                    ParticleManager.ProjectileHit.AddParticles(AmmoManager.LastBulletPos + CameraManager.Camera.Pos);
                }

            }
        }
    }
}
