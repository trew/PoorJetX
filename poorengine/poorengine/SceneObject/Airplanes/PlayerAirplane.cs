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
        private bool _bombPathTrigger;

        private Weapon _bombWeapon;
        public Weapon BombWeapon { get { return _bombWeapon; } }
        private Weapon _projectileWeapon;
        public Weapon ProjectileWeapon { get { return _projectileWeapon; } }

        private double _maxThrust = 7;

        public PlayerAirplane():
            base(2000, "Player/airplane_player")
        {
            _bombWeapon = new BombWeapon(this);
            _projectileWeapon = new ProjectileWeapon(this);
        }

        public override void HandleDebugInput(Input input)
        {
            if (input.IsNewKeyPress(Keys.Y))
            {
                TakeDamage(200);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                _thrust = 0;
            }

            if (input.IsNewKeyPress(Keys.O))
            {
                ParticleManager.Explosion.AddParticles(CameraManager.Camera.Pos + new Vector2(GameHelper.HalfScreenWidth + 100, GameHelper.ScreenHeight - 30));
            }
        }

        public override void AirExplode()
        {
            CameraManager.Camera.Stop(Position);
            base.AirExplode();
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (IsCrashing) return;
            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Projectile)))
            {
                Projectile proj = (Projectile)collidingWith;
                //TakeDamage(proj.Damage);
            }
            else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(EnemyAirplane)))
            {
                IsDead = true;
                IsCrashing = true;
                _health = 0;
                AirExplode();
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsDead && !CameraManager.Camera.Stopped)
                CameraManager.Camera.Stop(Position);
            else if (LevelManager.CurrentLevel.Completed)
            {
                CameraManager.Camera.Cruise();
            }
        }

        protected override void UpdatePhysics(GameTime gameTime)
        {
            base.UpdatePhysics(gameTime);
            if (LevelManager.CurrentLevel.Completed)
            {
                // Force behavior
                if (_orientation < 180 && _orientation > 90)
                {
                    _orientation -= 1f;
                    if (CalcHelper.formatAngle(_orientation) < 90) _orientation = 90;
                }
                else
                {
                    _orientation += 1f;
                    if (_orientation < 180 && _orientation > 90) _orientation = 90;
                }
                _thrust = _maxThrust;
            }

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
            float ymod;
            if (!LevelManager.CurrentLevel.Completed)
            {
                ymod = (float)(((newY * movementMultiplier)) + _gravity - _lift);
                ymod += (float)(9.8 * gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
                ymod = (float)(newY * movementMultiplier);
            }

            
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

        public override void Draw(GameTime gameTime)
        {
            drawBulletPath();
            if (_bombPathTrigger) drawBombPath();

            base.Draw(gameTime);
        }

        private void drawBulletPath()
        {
            Vector2 oldPoint = CalcHelper.calculatePoint(Position, Orientation - 10, 30f);
            Vector2 newPoint = CalcHelper.calculatePoint(Position, Orientation - 10, 30f);

            float xFactor = (float)Math.Sin(CalcHelper.DegreeToRadian(_orientation));
            float yFactor = (float)-Math.Cos(CalcHelper.DegreeToRadian(_orientation));
            Vector2 boostFactor = new Vector2(xFactor * 15, yFactor * 15);
            Vector2 pathbullet_velocity = _velocity + boostFactor;
            ScreenManager.SpriteBatch.Begin();
            
            for (int i = 1; i < 7; i++)
            {
                pathbullet_velocity += new Vector2(0, (float)(6.3 * EngineManager.Game.TargetElapsedTime.TotalSeconds * i));

                oldPoint = newPoint;
                newPoint += pathbullet_velocity * i;

                DrawLine(Color.Black * (float)(0.3f / (float)i), oldPoint, newPoint);

                if (newPoint.Y > GameHelper.GroundLevel + 10)
                    break;
            }

            ScreenManager.SpriteBatch.End();
        }

        private void drawBombPath()
        {
            Texture2D tex = TextureManager.GetTexture("bombtargetmarker").BaseTexture as Texture2D;

            Vector2 oldPoint = CalcHelper.calculatePoint(Position, Orientation + 90, 10f);
            Vector2 newPoint = oldPoint;
            Vector2 pathbomb_velocity = _velocity;// +boostFactor;
            double timeFactor = EngineManager.Game.TargetElapsedTime.TotalSeconds;
            int screenHeight = GameHelper.ScreenHeight;

            ScreenManager.SpriteBatch.Begin();

            float precision = 2f; // higher precision = more detailed drawn path, however shorter path
            int repeats = 40; // higher number of repeats = longer path. TO HIGH GIVES BAD PERFORMANCE!

            for (int i = 1; i < repeats; i++)
            {
                pathbomb_velocity += new Vector2(0, (float)((5.8f) * timeFactor / precision * (i)));
                oldPoint = newPoint;
                newPoint += pathbomb_velocity / precision * i;

                if (newPoint.Y < GameHelper.GroundLevel)
                    DrawLine(Color.Red * (float)(6f / (float)i) * 0.5f, oldPoint, newPoint);

                if (newPoint.Y > GameHelper.GroundLevel)
                {
                    double k = (oldPoint.Y - newPoint.Y) / -(oldPoint.X - newPoint.X);
                    
                    // x = (y-m)/k     We cannot afford to loose this formulare, after all our hard work with Gymnasiematte!
                    Vector2 drawPoint = new Vector2((float)(((oldPoint.Y) - GameHelper.GroundLevel) / k), GameHelper.GroundLevel);

                    Vector2 markerDrawPoint = new Vector2(CameraManager.Camera.Normalize(oldPoint + drawPoint).X,
                            GameHelper.GroundLevel - 35f - CameraManager.Camera.Pos.Y)
                            + new Vector2(-tex.Width / 2, 0);

                    DrawLine(Color.Red * (float)(6f / (float)i) * 0.5f,
                        oldPoint,
                        new Vector2((oldPoint + drawPoint).X,
                            GameHelper.GroundLevel));

                    ScreenManager.SpriteBatch.Draw(
                        tex,
                        markerDrawPoint,
                        Color.White);
                    break;
                }
            }

            ScreenManager.SpriteBatch.End();
        }

        public void DrawLine(Color color, Vector2 start, Vector2 end)
        {
           ScreenManager.SpriteBatch.Draw(TextureManager.GetColorTexture(color), 
               CameraManager.Camera.Normalize(start), 
               null, 
               Color.White,
               (float)Math.Atan2(end.Y - start.Y, end.X - start.X),
                Vector2.Zero,
                new Vector2(Vector2.Distance(start, end), 1f),
                SpriteEffects.None, 0f);
        }


        public override void HandleInput(Input input)
        {
            HandleDebugInput(input);

            if (IsCrashing) return;

            double forceIncreaseAmount = MathHelper.Clamp((float)(0.1 / _linearVelocity), 0.002f, 0.4f);
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
                _thrust = CalcHelper.Clamp(_thrust + 0.05, 0.0, _maxThrust);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
            {
                _thrust = CalcHelper.Clamp(_thrust - 0.05, 0.0, _maxThrust);
            }

            EngineManager.Debug.Print("COBRA FORCE: " + _cobraForce); 
            if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                _cobraForce = MathHelper.Clamp((float)(_cobraForce -= 0.021f), 0f, 4f);
                _orientation -= _cobraForce;
                ParticleManager.WhiteSmoke.AddParticles(CalcHelper.calculatePoint(Position, (float)_orientation + 180f, 30f));
                ParticleManager.WhiteSmoke.AddParticles(CalcHelper.calculatePoint(Position, (float)_orientation + 80f, 25f));
            }
            else
            {
                _cobraForce = MathHelper.Clamp((float)(_cobraForce += 0.03f), 0f, 4f);
                
            }

            if (_bombPathTrigger && input.CurrentKeyboardState.IsKeyUp(Keys.Space))
            {
                BombWeapon.Fire();
                _bombPathTrigger = false;
            }
            else if (input.CurrentKeyboardState.IsKeyDown(Keys.Space))
            {
                _bombPathTrigger = true;
            }

            if (input.LastKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                ProjectileWeapon.Fire();
            }
        }
    }
}
