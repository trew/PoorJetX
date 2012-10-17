using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using Microsoft.Xna.Framework;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class Projectile : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private IPoorSceneObject _originator;
        Vector2 _velocity;
        float _orientation;
        Random rnd;
        double _spread;
        public int Damage;
        int _soundFX_id;

        public double SpawnTime { get; set; }
        private double _invulnerableTime;

        public enum ProjectileType
        {
            Bullet,
            Bomb
        };

        private ProjectileType _type;
        public ProjectileType Type
        { 
            get { return _type; }
        }


        public Projectile(Vector2 pos, Vector2 velocity, string texture, float scale, IPoorSceneObject origin):
            base(texture)
        {
            /*
             *    THIS IS THE MOTHERFUCKING BOOOOMBS! edit: can also be used as airplane-urine
             */
            _type = ProjectileType.Bomb;

            rnd = new Random(); // remove?
            
            Position = pos;
            _velocity = velocity;
            _orientation = 0f;
            _spread = 0.0;
            _soundFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("bombwhistle"));
            SoundFxManager.GetByID(_soundFX_id).IsLooped = true;

            Scale = new Vector2(scale, scale);
            Z = 1.5f;
            Damage = 200;
            SpawnTime = 0.0;
            UsedInBoundingBoxCheck = true;
            _originator = origin;
            _invulnerableTime = 1.0f;
        }

        public Projectile(Vector2 pos, Vector2 velocity, float velocityBoost, float orientation, float spreadDegrees, string texture, IPoorSceneObject origin):
            base(texture)
        {
            /*
             *    THIS IS THE MOTHERFUCKING SHOTS!
             */
            _type = ProjectileType.Bullet;

            Damage = 100;
            rnd = new Random(Guid.NewGuid().GetHashCode());
            _spread = (rnd.NextDouble() * spreadDegrees) / 2.0;

            if (rnd.NextDouble() > 0.5)
            {
                _spread = -_spread;
            }
            _orientation = orientation + (float)_spread;

            Position = pos;
            float xFactor = (float)Math.Sin(CalcHelper.DegreeToRadian(_orientation));
            float yFactor = (float)-Math.Cos(CalcHelper.DegreeToRadian(_orientation));
            Vector2 boostFactor = new Vector2(xFactor * velocityBoost, yFactor * velocityBoost);

            _velocity = velocity + boostFactor;
            Z = 1.5f;
            SpawnTime = 0.0f;
            UsedInBoundingBoxCheck = true;
            _originator = origin;
            _invulnerableTime = 0.1f;
        }


        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 _origin = new Vector2(texture.Width / 2, texture.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                            CameraManager.Camera.Normalize(Position), null, Color.AliceBlue,
                                            _orientation + (float)CalcHelper.DegreeToRadian(180), _origin, Scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();

        }

        public void Update(GameTime gameTime)
        {
            _velocity += new Vector2(0, (float)(5.8 * gameTime.ElapsedGameTime.TotalSeconds));

            // Lets whistle!
            if (_type == ProjectileType.Bomb && _velocity.Y > 4)
            {
                if(SoundFxManager.GetByID(_soundFX_id).State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                    SoundFxManager.GetByID(_soundFX_id).Play();

                SoundFxManager.GetByID(_soundFX_id).Pan = CalcHelper.CalcPan(Position).X * 1.7f;

                float pitch =  1f - ((_velocity.Y - 4f) /10f); // Epic formula. Great taste. Crunchy on the outside. Chewy in the middle!
                SoundFxManager.GetByID(_soundFX_id).Pitch = MathHelper.Clamp(pitch - 0.3f , -1, 1);
                SoundFxManager.GetByID(_soundFX_id).Volume = CalcHelper.CalcVolume(Position) * MathHelper.Clamp(0.85f - pitch, 0f, 1f) * 0.3f;
            }

            _orientation = (float)CalcHelper.getAngleAsRadian(Position, Position + _velocity);
            Position += _velocity;
            if (SpawnTime <= 0.0)
                SpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;

            if (Position.Y > EngineManager.Device.Viewport.Height)
            {
                if (_type == ProjectileType.Bullet)
                {
                    SoundFxLibrary.GetFx("hitplane1").Play(CalcHelper.CalcVolume(Position) * 0.05f, CalcHelper.RandomBetween(-1.0f, -0.4f), CalcHelper.CalcPan(Position).X * 1.5f);
                    SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position - new Vector2(0, 15), 0f, new Vector2(0.2f, 0.2f), 200, 50, false, 0.9f));
                }
                else if (_type == ProjectileType.Bomb)
                {
                    ParticleManager.GroundExplosion.AddParticles(Position, 0, 35);
                    SoundFxLibrary.GetFx("bomb2").Play(CalcHelper.CalcVolume(Position) * 0.35f, CalcHelper.RandomBetween(-0.5f, 1f), CalcHelper.CalcPan(Position).X * 1.2f);

                    // Remove whisteling sound
                    SoundFxManager.GetByID(_soundFX_id).Stop();
                    SoundFxManager.RemoveFx(_soundFX_id);
                }


                SceneGraphManager.RemoveObject(this);
            }
        }


        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (collidingWith.GetType() == typeof(PlayerAirplane)) {
                SoundFxManager.RemoveFx(_soundFX_id);
                SceneGraphManager.RemoveObject(this);
            } 
            else if (collidingWith.GetType() == typeof(EnemyAirplane))
            {
                SoundFxManager.RemoveFx(_soundFX_id);
                SceneGraphManager.RemoveObject(this);
            }
        }

        public bool CanCollideWithObject(GameTime gameTime, IPoorSceneObject obj)
        {
            if (obj == _originator)
                return (gameTime.TotalGameTime.TotalSeconds > SpawnTime + _invulnerableTime);
            return true;
        }
   
    }
}
