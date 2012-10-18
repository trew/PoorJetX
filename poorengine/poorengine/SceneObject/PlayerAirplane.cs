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

        public override void Draw(GameTime gameTime)
        {
            drawBulletPath();

            drawBombPath();
            /*
            if (DateTime.Now.Second % 2 == 0)
                drawBombPath();
            else
                drawBombPath2();
            */
            base.Draw(gameTime);
        }

        private void drawBulletPath()
        {
            Vector2 oldPoint = Position;
            Vector2 newPoint = Position;

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

                DrawLine(Color.Black * (float)(0.5f / (float)i), oldPoint, newPoint);

            }

            ScreenManager.SpriteBatch.End();
        }

        private void drawBombPath()
        {
            if (!_bombPathTrigger) return;
            Texture2D tex = TextureManager.GetTexture("bombtargetmarker").BaseTexture as Texture2D;

            Vector2 oldPoint = CalcHelper.calculatePoint(Position, Orientation + 90, 10f);
            Vector2 newPoint = oldPoint;
            Vector2 pathbomb_velocity = _velocity;// +boostFactor;
            double timeFactor = EngineManager.Game.TargetElapsedTime.TotalSeconds;
            int screenHeight = EngineManager.Device.Viewport.Height;

            ScreenManager.SpriteBatch.Begin();

            float precision = 2f; // higher precision = more detailed drawn path, however shorter path
            int repeats = 50; // higher number of repeats = longer path. TO HIGH GIVES BAD PERFORMANCE!

            for (int i = 1; i < repeats; i++)
            {
                pathbomb_velocity += new Vector2(0, (float)((5.8f) * timeFactor / precision * (i)));
                oldPoint = newPoint;
                newPoint += pathbomb_velocity / precision * i;

                //DrawLine(Color.Red * (float)(20f / (float)i), oldPoint, newPoint);
                DrawLine(Color.White, oldPoint, newPoint);
                if (newPoint.Y > screenHeight - 30)
                {

                    double k = (oldPoint.Y - newPoint.Y) / -(oldPoint.X - newPoint.X);
                    EngineManager.Debug.Print("  K: " + k);
                    /* y = k x + m

                    y  -m = k x
                     
                    x = (y-m)/k
                      
                    */ 
                      
                    //  x = (20 - newPoint.Y) / k
                    Vector2 drawPoint = new Vector2((float)(((oldPoint.Y) - (screenHeight - 30)) / k), screenHeight - 30f);
                    EngineManager.Debug.Print(" DRAWING AT: " + (oldPoint + drawPoint));

                    ScreenManager.SpriteBatch.Draw(
                        tex,
                        new Vector2(CameraManager.Camera.Normalize(oldPoint + drawPoint).X, screenHeight - 50f)
                            + new Vector2(-tex.Width / 2, 0),
                        Color.White);

                    /*ScreenManager.SpriteBatch.Draw(
                        TextureManager.GetTexture("apTex1").BaseTexture as Texture2D,
                        new Vector2(CameraManager.Camera.Normalize(newPoint).X, screenHeight - 50f)
                            + new Vector2(-tex.Width / 2, 0),
                        Color.White);*/
                    break;
                }

            }

            ScreenManager.SpriteBatch.End();
        }



        private void drawBombPath2()
        {
            Texture2D tex = TextureManager.GetTexture("bombtargetmarker").BaseTexture as Texture2D;

            Vector2 oldPoint = CalcHelper.calculatePoint(Position, Orientation + 90, 10f);
            Vector2 newPoint = oldPoint;
            Vector2 pathbomb_velocity = _velocity;// +boostFactor;
            double timeFactor = EngineManager.Game.TargetElapsedTime.TotalSeconds;
            int screenHeight = EngineManager.Device.Viewport.Height;

            ScreenManager.SpriteBatch.Begin();

            float precision = 5f; // higher precision = more detailed drawn path, however shorter path
            int repeats = 35; // higher number of repeats = longer path. TO HIGH GIVES BAD PERFORMANCE!

            for (int i = 1; i < repeats; i++)
            {
                pathbomb_velocity += new Vector2(0, (float)((5.8f) * timeFactor / precision * (i)));
                oldPoint = newPoint;
                newPoint += pathbomb_velocity / precision * i;

                //DrawLine(Color.Red * (float)(20f / (float)i), oldPoint, newPoint);
                DrawLine(Color.Red, oldPoint, newPoint);
                if (newPoint.Y > screenHeight - 50)
                {
                    ScreenManager.SpriteBatch.Draw(
                        tex,
                        new Vector2(CameraManager.Camera.Normalize(newPoint).X,
                            EngineManager.Device.Viewport.Height - 60)
                            + new Vector2(-tex.Width / 2, 0),
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

            /* THIS ... *//*
            if (_bombPathTrigger && input.IsNewKeyPress(Keys.Space)) {
                if (AmmoManager.dropBomb())
                {
                    SoundFxLibrary.GetFx("bombdrop").Play(0.3f, CalcHelper.RandomBetween(0.8f, 0.2f), CalcHelper.CalcPan(Position).X * 1.8f);
                    SceneGraphManager.AddObject(new BombProjectile(CalcHelper.calculatePoint(Position, Orientation + 90, 10f), Velocity, this));
                    _bombPathTrigger = false;
                }
            } else if (input.IsNewKeyPress(Keys.Space))
            {
                _bombPathTrigger = true;
            }
            *//* ... OR THIS */
            if (_bombPathTrigger && input.CurrentKeyboardState.IsKeyUp(Keys.Space))
            {
                if (AmmoManager.dropBomb())
                {
                    SoundFxLibrary.GetFx("bombdrop").Play(0.3f, CalcHelper.RandomBetween(0.8f, 0.2f), CalcHelper.CalcPan(Position).X * 1.8f);
                    SceneGraphManager.AddObject(new BombProjectile(CalcHelper.calculatePoint(Position, Orientation + 90, 10f), Velocity, this));
                    _bombPathTrigger = false;
                }
            }
            else if (input.IsNewKeyPress(Keys.Space))
            {
                _bombPathTrigger = true;
            }/**/

            if (input.LastKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (AmmoManager.fireBullet())
                {

                    SoundFxLibrary.GetFx("firebullet").Play(
                                                            0.1f,
                                                            CalcHelper.RandomBetween(-0.2f, 0.3f),
                                                            CalcHelper.CalcPan(Position).X );

                    SceneGraphManager.AddObject(new BulletProjectile(CalcHelper.calculatePoint(Position, Orientation, 30f), Velocity, 15f, Orientation, 3f, this));
                    ParticleManager.ProjectileHit.AddParticles(AmmoManager.LastBulletPos + CameraManager.Camera.Pos);
                }

            }
        }
    }
}
