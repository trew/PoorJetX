using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.GameComponents;
using PoorEngine.Helpers;
using PoorEngine.Particles;

namespace PoorEngine.SceneObject
{
    public abstract class Airplane : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable, IPoorWeaponHolder
    {
        protected double _thrust;
        protected double _airSpeed;
        protected Vector2 _oldPos;
        protected double _orientation;
        protected double _lforce;
        protected double _rforce;
        protected double _cobraForce;
        protected double _lift;
        protected double _gravity;
        protected Vector2 _velocity;
        protected double _velocityAngle;
        protected double _linearVelocity;
        protected double _weight;
        protected double _angleOfAttack;
        protected double _angleSpeedModifier;

        public bool IsDead { get; set; }
        public bool IsCrashing { get; set; }
        
        protected int _health;
        protected int _maxHealth;

        protected Rectangle _hpRectOutline;
        protected Rectangle _healthMeterRect;

        protected int _smokeTimer;
        protected int _smokeTimerStartVal;

        protected ProjectileHit _fireeeee;
        protected BlackSmoke _blackSmoke;
        protected WhiteSmoke _whiteSmoke;
        protected GroundDust _groundDust;

        // Sounds
        protected int _engineFX_id;
        protected int _diveFX_id;
        protected int _fireBulletFX_id;

        public Airplane(int maxHealth, string textureName):
            base(textureName, textureName + "_destroyed")
        {
            _thrust = 4;
            _lift = 0;
            _orientation = 90;
            _airSpeed = 4;
            _gravity = 3;
            _linearVelocity = 0;
            _velocityAngle = 90;
            _weight = 1;
            _fireeeee = new ProjectileHit(EngineManager.Game, 7);
            _blackSmoke = new BlackSmoke(EngineManager.Game, 8);
            _whiteSmoke = new WhiteSmoke(EngineManager.Game, 8);
            _groundDust = new GroundDust(EngineManager.Game, 5);

            EngineManager.Game.Components.Add(_whiteSmoke);
            EngineManager.Game.Components.Add(_blackSmoke);
            EngineManager.Game.Components.Add(_fireeeee);
            EngineManager.Game.Components.Add(_groundDust);

            Position = new Vector2(200, 500);
            Z = 0.997f;
            UsedInBoundingBoxCheck = true;
            IsDead = false;
            IsCrashing = false;
            _health = this._maxHealth = maxHealth;

            _hpRectOutline = new Rectangle(9999, 9999, 40, 5);
            _healthMeterRect = new Rectangle(9999, 9999, 38, 3);
            _smokeTimer = 1;
            _smokeTimerStartVal = 20;

        }

        public virtual void Reset()
        {
            _thrust = 4;
            _lift = 0;
            _orientation = 90;
            _airSpeed = 4;
            _gravity = 3;
            _linearVelocity = 0;
            _velocityAngle = 90;
            _weight = 1;
            Position = new Vector2(200, 500);
            Z = 0.997f;
            UsedInBoundingBoxCheck = true;
            IsDead = false;
            IsCrashing = false;
            _health = this._maxHealth;

            _smokeTimer = 1;
            _smokeTimerStartVal = 20;

        }

        public override Rectangle BoundingBox
        { 
            get
            {
                Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
                if ((_orientation >= -5 && _orientation < 45) || (_orientation > 315 && _orientation <= 365) || (_orientation > 135 && _orientation < 225))
                {
                    return new Rectangle(
                        (int)Position.X - texture.Height / 2 + 5,
                        (int)Position.Y - texture.Width / 2  + 5,
                        texture.Height - 10,
                        texture.Width - 10
                    );
                }

                return new Rectangle(
                        (int)Position.X - texture.Width / 2 + 5,
                        (int)Position.Y - texture.Height / 2 + 5,
                        texture.Width - 10,
                        texture.Height - 10
                    );
            }
        }

        public double LinearVelocity { get { return _linearVelocity; } }
        public double Thrust { get { return _thrust; } }
        public float Orientation { get { return (float)_orientation; } }
        public Vector2 Velocity { get { return _velocity; } }
        public float HitPointsPercent { get { return ((float)_health / _maxHealth); } }
        public bool HeadingRight { get { return _velocity.X > 0; } }
        public bool HeadingDown { get { return _velocity.Y > 0; } }

        public virtual void HandleDebugInput(Input input) {}

        public virtual void TakeDamage(int dmg)
        {
            _health -= dmg;
            if (_health <= 0)
            {
                IsCrashing = true;
                _health = 0;
            }
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (IsCrashing) return;

            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Projectile)))
            {
                Projectile proj = (Projectile)collidingWith;
                TakeDamage(proj.Damage);
            }
            else if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(GroundVehicle)))
            {
                IsDead = true;
                IsCrashing = true;
                _health = 0;
                AirExplode();
            }

        }

        public virtual void Kill()
        {
            _health = 0;
            IsCrashing = true;
            if (SceneGraphManager.TypeMatch(this.GetType(), typeof(IPoorEnemy)))
                EngineManager.Score += 1;
        }

        public void GroundExplode()
        {
            //SceneGraphManager.RemoveObject(this);
            UsedInBoundingBoxCheck = false;

            SoundFxManager.RemoveFx(_diveFX_id);
            SoundFxManager.RemoveFx(_engineFX_id);
            SoundFxManager.RemoveFx(_fireBulletFX_id);

            _orientation = CalcHelper.RandomBetween(100f, 140f);

            SoundFxLibrary.GetFx("bomb1").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.4f), CalcHelper.RandomBetween(0f, 0.4f), CalcHelper.CalcPan(Position).X * 1.8f);

            SceneGraphManager.AddObject(new AnimatedSprite("anim_groundcrash", new Point(300, 150), new Point(12, 10), Position + new Vector2(150, -80), 0f, new Vector2(2f, 2f), 200, 100, false, 0.9f));
            ParticleManager.GroundExplosion.AddParticles(Position, _velocity.X * 8f, 10f);
            ParticleManager.ShrapnelExplosion.AddParticles(Position, 0, 150);
        }

        public virtual void AirExplode()
        {
            SceneGraphManager.RemoveObject(this);
            UsedInBoundingBoxCheck = false;

            SoundFxManager.RemoveFx(_diveFX_id);
            SoundFxManager.RemoveFx(_engineFX_id);
            SoundFxManager.RemoveFx(_fireBulletFX_id);

            SoundFxLibrary.GetFx("bomb2").Play(SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.4f),
                                    CalcHelper.RandomBetween(0f, 0.4f), CalcHelper.CalcPan(Position).X * 1.8f);

            ParticleManager.Explosion.AddParticles(Position, 0f, 360f);
            ParticleManager.ShrapnelExplosion.AddParticles(Position, 0f, 360f);
        }

        public virtual void Draw(GameTime gameTime)
        {
            Texture2D texture;

            if (IsDead)
                texture = TextureManager.GetTexture(TextureNameDestroyed).BaseTexture as Texture2D;
            else
                texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           CameraManager.Camera.Normalize(Position), null, Color.AliceBlue,
                                           (float)CalcHelper.DegreeToRadian(_orientation - 90),
                                           origin, Scale, SpriteEffects.None, 0f);

            // Draw _health-bar, if plane still alive
            if (!(IsCrashing || IsDead))
            {
                // Update Healthbar draw-settings.
                _healthMeterRect.Width = (int)(38 * ((float)_health / _maxHealth));
                int red = (int)(255 - 255 * HitPointsPercent);
                int green = (int)(255 * HitPointsPercent);

                Color hpColor = new Color(red * 3, green * 2, 0);

                Texture2D texBlack = TextureManager.GetColorTexture(Color.Black);
                Texture2D texHealth = TextureManager.GetColorTexture(hpColor);
                ScreenManager.SpriteBatch.Draw(texBlack,
                                               CameraManager.Camera.Normalize(Position) + new Vector2(-10, 20),
                                               _hpRectOutline,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
                ScreenManager.SpriteBatch.Draw(texHealth,
                                               CameraManager.Camera.Normalize(Position) + new Vector2(-9, 21),
                                               _healthMeterRect,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
            }

            if (EngineManager.Debug.ViewDebug) // Draw debug boundingbox
                TextureManager.DrawRectangle(ScreenManager.SpriteBatch, CameraManager.Camera.Normalize(BoundingBox), 1, Color.Black);

            ScreenManager.SpriteBatch.End();
        }

        public virtual void Update(GameTime gameTime)
        {
            EngineManager.Device.Textures[0] = null;

            if (Position.Y > GameHelper.GroundLevel -10 && !IsDead)
            {
                Kill();
                GroundExplode();
                IsDead = true;
                return;
            }

            if (!IsDead)
            {
                if (Position.Y > GameHelper.GroundLevel - 45)
                {
                    AddGroundDust();
                }

                // Spawn smoke-effects if HP is low
                if (HitPointsPercent < 0.7)
                {
                    _smokeTimerStartVal = Math.Max(5, (int)(50 * HitPointsPercent));
                    _smokeTimer--;

                    if (_smokeTimer <= 0)
                    {
                        _blackSmoke.AddParticles(CalcHelper.calculatePoint(Position, Orientation + 10, 35f));
                        _smokeTimer = _smokeTimerStartVal;

                        if (HitPointsPercent < 0.15)
                            _fireeeee.AddParticles(CalcHelper.calculatePoint(Position, Orientation + 10, 35f));
                    }
                }

                // Execute airplane-crash. Damn i love this comment.
                // Damn good bloody damn good job.
                // You're an excellent woman!
                // In fact, you're the best person here!
                if (IsCrashing)
                {
                    _blackSmoke.AddParticles(CalcHelper.calculatePoint(Position, Orientation + 10, 15f));
                    _fireeeee.AddParticles(CalcHelper.calculatePoint(Position, Orientation + 10, 35f));
                    _orientation = Math.Min(150, _orientation + 0.3);
                }
                UpdatePhysics(gameTime);

                // Update soundFX based on _airSpeed-calculations etc
                UpdateSound();
            }
        }

        protected virtual void UpdatePhysics(GameTime gameTime)
        {
        }

        public virtual void UpdateSound()
        {
            if (!IsDead)
            {
                float enginePitch = (float)(Math.Pow((_thrust / 9), 1.8) - 0.1f);
                enginePitch += (float)(_linearVelocity / 15);
                SoundFxManager.GetByID(_engineFX_id).Pitch = MathHelper.Clamp(enginePitch, -1f, 1f);
                float volume = MathHelper.Clamp(enginePitch, 0.6f, 1f) * CalcHelper.CalcVolume(Position);
                SoundFxManager.GetByID(_engineFX_id).Volume = SoundFxManager.GetVolume("Sound", volume);

                // Add dive-sound if speed is high enough
                float airspeedPitch;
                float diveVolume = 0;
                if (_linearVelocity > 7)
                {
                    airspeedPitch = (float)((_linearVelocity - 7) / 7);
                    airspeedPitch -= 0.1f;
                    diveVolume = MathHelper.Clamp(airspeedPitch, 0f, 1f);
                    SoundFxManager.GetByID(_diveFX_id).Pitch = MathHelper.Clamp(airspeedPitch, -1f, 1f);
                }
                SoundFxManager.GetByID(_diveFX_id).Volume = SoundFxManager.GetVolume("Sound", diveVolume);

                SoundFxManager.GetByID(_engineFX_id).Pan = CalcHelper.CalcPan(Position).X;
                SoundFxManager.GetByID(_diveFX_id).Pan = CalcHelper.CalcPan(Position).X;
            }
        }

        public virtual void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/" + TextureName), TextureName);
            TextureManager.AddTexture(new PoorTexture("Textures/" + TextureNameDestroyed), TextureNameDestroyed);
            
            SoundFxLibrary.AddToLibrary("SoundFX/engine1", "engine1");
            SoundFxLibrary.AddToLibrary("SoundFX/dive1", "dive1");
            SoundFxLibrary.AddToLibrary("SoundFX/firebullet", "firebullet");

            _engineFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("engine1"));
            _diveFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("dive1"));
            _fireBulletFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("firebullet"));

            SoundFxManager.GetByID(_engineFX_id).Volume = SoundFxManager.GetVolume("Sound", 0.3f);
            SoundFxManager.GetByID(_engineFX_id).IsLooped = true;
            SoundFxManager.GetByID(_engineFX_id).Play();

            SoundFxManager.GetByID(_diveFX_id).IsLooped = true;
            SoundFxManager.GetByID(_diveFX_id).Volume = 0f;
            SoundFxManager.GetByID(_diveFX_id).Play();
        }

        public virtual void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
            TextureManager.RemoveTexture(TextureNameDestroyed);
        }

        public virtual void HandleInput(Input input)
        {
            HandleDebugInput(input);
        }

        protected void drawWingtipVortices()
        {
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1",
                                                           new Point(100, 100),
                                                           new Point(10, 1),
                                                           Position + new Vector2(0, -4),
                                                           (float)CalcHelper.DegreeToRadian(_orientation),
                                                           new Vector2(0.1f, 0.8f),
                                                           250,
                                                           5,
                                                           false,
                                                           0.9f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1",
                                                           new Point(100, 100),
                                                           new Point(10, 1),
                                                           Position + new Vector2(2, 10),
                                                           (float)CalcHelper.DegreeToRadian(_orientation),
                                                           new Vector2(0.1f, 0.8f),
                                                           250,
                                                           5,
                                                           false,
                                                           0.9f));
        }

        private void AddGroundDust()
        {
            EngineManager.Debug.Print("Linear velocity: " + LinearVelocity);
            

            if (LinearVelocity > 5)
                _groundDust.AddParticles(
                    new Vector2(
                        (Position.X - (GameHelper.GroundLevel - Position.Y)),
                        GameHelper.GroundLevel), 45f, 20f);

            if (LinearVelocity > 6.5)
                _whiteSmoke.AddParticles(
                    new Vector2(
                        (Position.X - (GameHelper.GroundLevel - Position.Y)),
                        GameHelper.GroundLevel), 45f, 20f);
        }
    }
}
