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
    public class EnemyAirplane : Airplane, IPoorEnemy
    {
        private Airplane _target;
        public Airplane Target { get { return _target; } }

        public bool HasTarget { get { return _target != null; } }

        private bool _requiredForVictory;
        public bool RequiredForVictory { get { return _requiredForVictory; } }

        private Cannon _weapon;

        private string RndEnemyTex()
        {
            return "Enemies/airplane_enemy" + (int)CalcHelper.RandomBetween(1f, 5.99f);
        }

        public EnemyAirplane(int maxHealth, bool requiredForVictory):
            base(maxHealth, "")
        {
            _requiredForVictory = requiredForVictory;
            TextureName = RndEnemyTex();
            TextureNameDestroyed = TextureName + "_destroyed";

            _thrust = 3;
            _airSpeed = 3;
            _weapon = new Cannon(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsCrashing) UpdateAI(gameTime);
            base.Update(gameTime);

            // DEBUG
            HandleInput(EngineManager.Input);
        }

        public void SetTarget()
        {
            if (CalcHelper.DistanceBetween(Position, EngineManager.Player.Position) < 800)
                _target = EngineManager.Player;
            else
                _target = null;
        }

        protected void AdjustThrottle(GameTime gameTime)
        {
            // Find Target and adjust throttle according to where 
            // target is located.
            if (Target == null)
            {
                Vector2 targetX = new Vector2(CameraManager.Camera.Pos.X + GameHelper.ScreenWidth * 0.8f, Position.Y);
                Vector2 targetDiff = Position - targetX;
                _thrust = MathHelper.Clamp((float)(2 - targetDiff.X / 300), 0f, 7f);
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
            int distanceMod = 1;// (int)CalcHelper.DistanceBetween(Position, Target.Position) / 75;
            double orientation = CalcHelper.getAngle(Position, CalcHelper.calculatePoint(Target.Position, Target.Orientation, (float)Target.LinearVelocity * distanceMod)) - 90f;

            return (float)CalcHelper.formatAngle(orientation);
        }

        protected void FireBullets(GameTime gameTime)
        {
            if (_target == null) return;

            _weapon.Angle = GetAngleToTarget(gameTime);

            _weapon.Fire();
        }

        protected void UpdateAI(GameTime gameTime)
        {
            SetTarget();
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
                    SoundFxLibrary.GetFx("hitplane1").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.1f),
                                        CalcHelper.RandomBetween(-0.5f, 0.1f), CalcHelper.CalcPan(Position).X * 1.8f);
                }

                else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(BombProjectile)))
                {
                    ParticleManager.ShrapnelExplosion.AddParticles(new Vector2(Position.X - 10, Position.Y), 0f, 360f);
                    ParticleManager.Explosion.AddParticles(Position);
                    SoundFxLibrary.GetFx("bomb4").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.7f),
                                        CalcHelper.RandomBetween(-0.5f, 0.2f), CalcHelper.CalcPan(Position).X * 1.8f);
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

            else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(GroundVehicle)))
            {
                if (IsCrashing)
                {
                    EngineManager.Score += 2;
                }

                ((GroundVehicle)collidingWith).TakeDamage(10000);
                _health = 0;
                AirExplode();
            }
        }
    }
}
