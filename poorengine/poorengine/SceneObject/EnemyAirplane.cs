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
    public class EnemyAirplane : Airplane
    {
        private Vector2 targetPos;
        private double targetX;

        public EnemyAirplane(int maxHealth):
            base(maxHealth, "apTex1")
        {
            _thrust = 3;
            _airSpeed = 3;
        }

        public void setTargetPos(Vector2 tp)
        {
            targetPos = tp;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // DEBUG
            HandleInput(EngineManager.Input);
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

            targetX = CameraManager.Camera.Pos.X + EngineManager.Device.Viewport.Width * 0.8;
            double xdiff = Position.X - targetX;
            _thrust = MathHelper.Clamp((float)(5 - xdiff / 300), 0f, 7f);

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

            double drag = Math.Sqrt(Math.Abs(_angleOfAttack) / 90);

            // Adds 'acceleration'
            if (_airSpeed < _thrust)
            {
                _airSpeed += 0.255;
            }
            else if (_airSpeed > _thrust)
            {
                _airSpeed -= 0.250;
            }

            double movementMultiplier = _airSpeed - drag;

            float xmod = (float)(newX * movementMultiplier);
            float ymod = (float)(newY * movementMultiplier);

            
            // Save old pos, used for speedcalculations
            _oldPos = Position;
            Position += new Vector2(xmod, ymod);
            _velocity = Position - _oldPos;

            _linearVelocity = Math.Sqrt(
                Math.Pow((Math.Max(Position.X, _oldPos.X) - Math.Min(Position.X, _oldPos.X)), 2) +
                Math.Pow((Math.Max(Position.Y, _oldPos.Y) - Math.Min(Position.Y, _oldPos.Y)), 2));

            if (!IsCrashing)
                _airSpeed = Math.Min(_airSpeed, 8);
            else
                _airSpeed = Math.Min(_airSpeed, 20);

        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (!IsCrashing && _health > 0 && collidingWith.GetType() == typeof(Projectile))
            {
                Projectile p = (Projectile)collidingWith;
                _health -= p.Damage;
                ParticleManager.ProjectileHit.AddParticles(new Vector2(Position.X-10, Position.Y));
                if (_health <= 0)
                {
                    _health = 0;
                    IsCrashing = true;
                    EngineManager.Score += 1;
                }
            } else if (collidingWith.GetType() == typeof(EnemyAirplane))
            {
                EnemyAirplane e = (EnemyAirplane)(collidingWith);

                // Is the other plane crashing?
                if (e.IsCrashing)
                {
                    EngineManager.Score += 2;
                }
                _health = 0;
                AirExplode();
            }
        }
    }
}
