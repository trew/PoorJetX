using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using Microsoft.Xna.Framework;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.SceneObject
{
    public class Projectile : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        Vector2 _velocity;
        float _orientation;
        Random rnd;
        double _spread;

        public Projectile(Vector2 pos, Vector2 velocity, string texture):
            base(texture)
        {
            rnd = new Random(); // remove?
            Position = pos;
            _velocity = velocity;
            _orientation = 0f;
            _spread = 0.0;
            Z = 1.5f;
            UsedInBoundingBoxCheck = true;
        }

        public Projectile(Vector2 pos, Vector2 velocity, float velocityBoost, float orientation, float spreadDegrees, string texture):
            base(texture)
        {
            rnd = new Random(Guid.NewGuid().GetHashCode());
            _spread = (rnd.NextDouble() * spreadDegrees) / 2.0;

            if (rnd.NextDouble() > 0.5)
            {
                _spread = -_spread;
            }
            _orientation = orientation + (float)_spread;

            Position = pos;
            float xFactor = (float)Math.Sin(DegreeToRadian(_orientation));
            float yFactor = (float)-Math.Cos(DegreeToRadian(_orientation));
            Vector2 boostFactor = new Vector2(xFactor * velocityBoost, yFactor * velocityBoost);

            _velocity = velocity + boostFactor;
            Z = 1.5f;
            UsedInBoundingBoxCheck = true;
        }


        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 _origin = new Vector2(texture.Width / 2, texture.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                            Position - CameraManager.Camera.Pos, null, Color.AliceBlue,
                                            _orientation + (float)DegreeToRadian(180), _origin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();

        }

        public void Update(GameTime gameTime)
        {
            if (true)
            {
                _velocity += new Vector2(0f, (float)(5.8 * gameTime.ElapsedGameTime.TotalSeconds));
            }

            _orientation = (float)getAngleAsRadian(Position, Position + _velocity);
            Position += _velocity;
        }


        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }


        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double getAngleAsRadian(Vector2 a, Vector2 b)
        {
            double deltax = a.X - b.X;
            double deltay = a.Y - b.Y;

            return Math.Atan2(deltay, deltax);
        }
   
    }
}
