using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Particles;
using PoorEngine.Managers;
using PoorEngine.Helpers;
using Microsoft.Xna.Framework;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.SceneObject
{
    public abstract class GroundVehicle : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        // Vehicle-status
        protected int _health;
        protected int _maxHealth;
        protected bool _destroyed;
        public float HitPointsPercent { get { return ((float)_health / _maxHealth); } }

        protected string _type;

        // Movement
        protected Vector2 _velocity;
        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        
        // Particle-effects
        protected ProjectileHit _fireeeee;
        protected BlackSmoke _blackSmoke;
        protected WhiteSmoke _whiteSmoke;

        //Health-bar
        protected Rectangle _hpRectOutline;
        protected Rectangle _healthMeterRect;

        // Sound-related
        protected int _engineFX_id;
        protected int _fireBulletFX_id;

        public GroundVehicle(int maxHealth, string textureName, string textureNameDestroyed)
            :base(textureName, textureNameDestroyed)
        {
            UsedInBoundingBoxCheck = true;
            Z = 0.999f;

            _destroyed = false;
            _health = _maxHealth = maxHealth;
            _hpRectOutline = new Rectangle(9999, 9999, 40, 5);
            _healthMeterRect = new Rectangle(9999, 9999, 38, 3);

            _fireeeee = new ProjectileHit(EngineManager.Game, 7);
            _blackSmoke = new BlackSmoke(EngineManager.Game, 8);
            _whiteSmoke = new WhiteSmoke(EngineManager.Game, 8);

            EngineManager.Game.Components.Add(_whiteSmoke);
            EngineManager.Game.Components.Add(_blackSmoke);
            EngineManager.Game.Components.Add(_fireeeee);
        }

        public virtual void Update(GameTime gameTime)
        {
            EngineManager.Device.Textures[0] = null;

            

            if (!_destroyed)
            {
                Position += _velocity;
                UpdateSound();
            }

            if (_destroyed && Position.X < CameraManager.Camera.Pos.X - 1000)
            {
                SoundFxManager.RemoveFx(_engineFX_id);
                SoundFxManager.RemoveFx(_fireBulletFX_id);
                SceneGraphManager.RemoveObject(this);
                return;
            }

            if (!_destroyed && Position.X < CameraManager.Camera.Pos.X - 2000)
            {
                SoundFxManager.RemoveFx(_engineFX_id);
                SoundFxManager.RemoveFx(_fireBulletFX_id);
                SceneGraphManager.RemoveObject(this);
                return;
            }

            

        }

        private void UpdateSound()
        {
            SoundFxManager.GetByID(_engineFX_id).Pan = CalcHelper.CalcPan(Position).X;
            SoundFxManager.GetByID(_engineFX_id).Volume = SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.3f);
        }

        public virtual void Draw(GameTime gameTime)
        {
            Texture2D texture;

            if(_destroyed)
                texture = TextureManager.GetTexture(TextureNameDestroyed).BaseTexture as Texture2D;
            else
                texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           CameraManager.Camera.Normalize(Position),  // Position
                                           null,                // Source-Rectangle 
                                           Color.White,
                                           0f,                 // Rotation
                                           Vector2.Zero,        // Origin 
                                           Scale, 
                                           SpriteEffects.None,
                                           0f);                 // Layer-depth

            // Draw _health-bar, if vehiclenot destroyed
            if (!_destroyed)
            {
                // Update Healthbar draw-settings.
                _healthMeterRect.Width = (int)(38 * ((float)_health / _maxHealth));
                int red = (int)(255 - 255 * HitPointsPercent);
                int green = (int)(255 * HitPointsPercent);

                Color hpColor = new Color(red * 3, green * 2, 0);

                Texture2D texBlack = TextureManager.GetColorTexture(Color.Black);
                Texture2D texHealth = TextureManager.GetColorTexture(hpColor);

                Vector2 normPos = CameraManager.Camera.Normalize(Position);
                Vector2 offset = new Vector2(40, 40);

                ScreenManager.SpriteBatch.Draw(texBlack,
                                               normPos + new Vector2(-10, 20) + offset,
                                               _hpRectOutline,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
                ScreenManager.SpriteBatch.Draw(texHealth,
                                               normPos + new Vector2(-9, 21) + offset,
                                               _healthMeterRect,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
            }
            
            ScreenManager.SpriteBatch.End();
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            
            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Projectile)))
            {
                Projectile proj = (Projectile)collidingWith;
                TakeDamage(proj.Damage);
            }

            if (SceneGraphManager.TypeMatch(collidingWith.GetType(), typeof(Airplane)))
            {
                TakeDamage(10000000);
                EngineManager.Score += 2; // Bonus points for being killed by crashing airplane
            }

            // EngineManager.Score += wahtever, om dör! Gör i underklasser ist?
          
        }

        public virtual void TakeDamage(int dmg)
        {
            _health -= dmg;
            if (_health <= 0)
            {
                _health = 0;
                _destroyed = true;
                GroundExplode();

                if (_type.Equals("battle"))
                    EngineManager.Score += 3;
                else if (_type.Equals("civilian"))
                    EngineManager.Score += 1;
                    
            }
        }

        public void GroundExplode()
        {
            UsedInBoundingBoxCheck = false;

            SoundFxManager.RemoveFx(_engineFX_id);
            SoundFxManager.RemoveFx(_fireBulletFX_id);

            SoundFxLibrary.GetFx("bomb1").Play(
                SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.3f), 
                CalcHelper.RandomBetween(0f, 0.4f), 
                CalcHelper.CalcPan(Position).X * 1.8f);

            SceneGraphManager.AddObject(new AnimatedSprite("anim_groundcrash", new Point(300, 150), new Point(12, 10), new Vector2(Position.X, GameHelper.ScreenHeight) + new Vector2(190, -160), 0f, new Vector2(2f, 2f), 200, 100, false, 0.9f));
            ParticleManager.GroundExplosion.AddParticles(new Vector2(Position.X + 40f, GameHelper.ScreenHeight - 70), 30f, 50f);
            ParticleManager.ShrapnelExplosion.AddParticles(new Vector2(Position.X + 40f, GameHelper.ScreenHeight - 70), 0f, 120f);
        }

        public virtual void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/Enemies/" + TextureName), TextureName);
            TextureManager.AddTexture(new PoorTexture("Textures/Enemies/" + TextureNameDestroyed), TextureNameDestroyed);

            SoundFxLibrary.AddToLibrary("SoundFX/engine1", "engine1");
            SoundFxLibrary.AddToLibrary("SoundFX/dive1", "dive1");
            SoundFxLibrary.AddToLibrary("SoundFX/firebullet", "firebullet");

            _engineFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("engine1"));
            _fireBulletFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("firebullet"));
            SoundFxManager.GetByID(_engineFX_id).Volume = SoundFxManager.GetVolume("Sound", CalcHelper.CalcVolume(Position) * 0.3f);
            SoundFxManager.GetByID(_engineFX_id).IsLooped = true;
            SoundFxManager.GetByID(_engineFX_id).Play();
        }

        public virtual void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
            TextureManager.RemoveTexture(TextureNameDestroyed);
        }
        
    }
}
