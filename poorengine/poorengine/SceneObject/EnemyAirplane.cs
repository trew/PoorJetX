using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
        private Airplane _target;
        public Airplane Target { get { return _target; } }

        private Stopwatch _reloadTimer;
        private Stopwatch _reloadBurstTimer;
        private int _firedBulletsInBurst;
        private bool _initialBurst = false; //Don't wait for the first burst
        const int BURSTSPERMINUTE = 10;
        const int BULLETSINBURST = 5;
        const int BURSTBULLETPERSECOND = 10;

        public EnemyAirplane(int maxHealth):
            base(maxHealth, "apTex1")
        {
            _thrust = 3;
            _airSpeed = 3;
            _reloadTimer = new Stopwatch();
            _reloadBurstTimer = new Stopwatch();
            _firedBulletsInBurst = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsCrashing) UpdateAI(gameTime);
            base.Update(gameTime);

            // DEBUG
            HandleInput(EngineManager.Input);
        }

        protected void SetTarget(GameTime gameTime)
        {
            if (_target == null)
                _target = EngineManager.Player;
        }

        protected void AdjustThrottle(GameTime gameTime)
        {
            // Find Target and adjust throttle according to where 
            // target is located.
            if (Target == null)
            {
                Vector2 targetX = new Vector2(CameraManager.Camera.Pos.X + EngineManager.Device.Viewport.Width * 0.8f, Position.Y);
                Vector2 targetDiff = Position - targetX;
                _thrust = MathHelper.Clamp((float)(5 - targetDiff.X / 300), 0f, 7f);
            }
            else
            {
                Vector2 targetX = Target.Position;
                Vector2 targetDiff = Position - targetX;
                _thrust = MathHelper.Clamp((float)(5 - targetDiff.X / 300), 0f, 7f);
            }
        }

        protected float GetAngleToTarget(GameTime gameTime)
        {
            int distanceMod = (int)CalcHelper.DistanceBetween(Position, Target.Position) / 75;
            double orientation = CalcHelper.getAngle(Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;

            //EngineManager.Debug.Print("Calc: " + (Target.Position.Y - Position.Y) / 100);
            orientation -= (Target.Position.X - Position.X) / 80;

            orientation += (Target.Position.Y - Position.Y) / 150;

            return (float)CalcHelper.formatAngle(orientation);
        }

        protected void FireBullets(GameTime gameTime)
        {
            if (_target == null) return;
            if (!_reloadTimer.IsRunning) _reloadTimer.Restart();

            GetAngleToTarget(gameTime);
            if (_reloadTimer.ElapsedMilliseconds > 60000 / BURSTSPERMINUTE || !_initialBurst)
            {
                // Start burst
                if (!_reloadBurstTimer.IsRunning) _reloadBurstTimer.Restart();

                if (_firedBulletsInBurst < BULLETSINBURST) {
                    if (_reloadBurstTimer.ElapsedMilliseconds > 1000 / BURSTBULLETPERSECOND) {
                        _reloadBurstTimer.Stop();
                        _firedBulletsInBurst++;
                        SoundFxLibrary.GetFx("firebullet").Play(
                                                        0.1f,
                                                        CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                        CalcHelper.CalcPan(Position).X);

                        float angle = GetAngleToTarget(gameTime);
                        SceneGraphManager.AddObject(new BulletProjectile(CalcHelper.calculatePoint(Position, Orientation, 30f),
                                                           Velocity, 15f,
                                                           angle, 3f,
                                                           this));
                    }

                // Stop the burst
                } else {
                    _firedBulletsInBurst = 0;
                    _initialBurst = true;
                    _reloadTimer.Restart();
                }
            }
        }

        protected void UpdateAI(GameTime gameTime)
        {
            SetTarget(gameTime);
            AdjustThrottle(gameTime);
            FireBullets(gameTime);
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
            if (!IsCrashing && _health > 0 && SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Projectile)))
            {
                Projectile p = (Projectile)collidingWith;
                _health -= p.Damage;
                
                if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(BulletProjectile)))
                {
                    ParticleManager.ProjectileHit.AddParticles(new Vector2(Position.X - 10, Position.Y));
                    SoundFxLibrary.GetFx("hitplane1").Play(CalcHelper.CalcVolume(Position) * 0.1f, CalcHelper.RandomBetween(-0.5f, 0.1f), CalcHelper.CalcPan(Position).X * 1.8f);
                }

                else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(BombProjectile)))
                {
                    ParticleManager.ShrapnelExplosion.AddParticles(new Vector2(Position.X - 10, Position.Y));
                    ParticleManager.Explosion.AddParticles(Position);
                    SoundFxLibrary.GetFx("bomb1").Play(CalcHelper.CalcVolume(Position) * 0.1f, CalcHelper.RandomBetween(-0.5f, 0.1f), CalcHelper.CalcPan(Position).X * 1.8f);
                }

                if (_health <= 0)
                {
                    _health = 0;
                    IsCrashing = true;
                    EngineManager.Score += 1;
                }
            } 
            else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Airplane)))
            {
                Airplane e = (Airplane)(collidingWith);

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
