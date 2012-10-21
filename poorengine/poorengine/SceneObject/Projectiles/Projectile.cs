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
    public abstract class Projectile : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        protected IPoorWeaponHolder _originator;
        protected Vector2 _velocity;
        protected float _orientation;
        
        protected Random rnd;
        public int Damage;

        public double SpawnTime { get; set; }
        protected double _invulnerableTime;

        public Projectile(string texture, Vector2 pos, Vector2 velocity, IPoorWeaponHolder origin) :
            base(texture)
        {
            Position = pos;
            _velocity = velocity;
            _orientation = 0f;

            Z = 1.5f;
            SpawnTime = 0.0;
            UsedInBoundingBoxCheck = true;
            _originator = origin;
            _invulnerableTime = 0;
        }

        public virtual void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 _origin = new Vector2(texture.Width / 2, texture.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                            CameraManager.Camera.Normalize(Position), null, Color.AliceBlue,
                                            _orientation + (float)CalcHelper.DegreeToRadian(180), _origin, Scale, SpriteEffects.None, 0f);

            if (EngineManager.Debug.ViewDebug) // Draw debug boundingbox
                TextureManager.DrawRectangle(ScreenManager.SpriteBatch, CameraManager.Camera.Normalize(BoundingBox), 1, Color.Black);

            ScreenManager.SpriteBatch.End();

        }

        public virtual void Update(GameTime gameTime)
        {
            if (SceneGraphManager.TypeMatch(typeof(PlayerAirplane), _originator.GetType()))
                _velocity += new Vector2(0, (float)(5.8 * gameTime.ElapsedGameTime.TotalSeconds));

            _orientation = (float)CalcHelper.getAngleAsRadian(Position, Position + _velocity);
            Position += _velocity;
            if (SpawnTime <= 0.0)
                SpawnTime = (float)gameTime.TotalGameTime.TotalSeconds;

            if (SpawnTime + 10.0 < gameTime.TotalGameTime.TotalSeconds) 
            {
                SceneGraphManager.RemoveObject(this);
            }
        }


        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            SceneGraphManager.RemoveObject(this);
        }

        public virtual bool CanCollideWithObject(GameTime gameTime, IPoorSceneObject obj)
        {
            if (obj == _originator)
                return (gameTime.TotalGameTime.TotalSeconds > SpawnTime + _invulnerableTime);
            return true;
        }
   
    }
}
