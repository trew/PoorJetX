using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class PoorSceneObject : IPoorSceneObject
    {
        public PoorSceneObject(String textureName)
        {
            TextureName = textureName;
        }

        public PoorSceneObject(String textureName, String textureNameDestroyed)
        {
            TextureName = textureName;
            TextureNameDestroyed = textureNameDestroyed;
        }

        public String TextureName { get; set; }
        public String TextureNameDestroyed { get; set; }

        public virtual void Collide(PoorSceneObject collidingWith) { }

        public void DrawArrow(string textureName, bool isArrow)
        {
            Vector2 pos = Position;
            float orientation = 0;
            float scale = 0.4f;
            // to the right
            Texture2D texture = TextureManager.GetTexture(textureName).BaseTexture as Texture2D;
            Texture2D targetTexture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            if (Position.X > CameraManager.Camera.Pos.X + GameHelper.ScreenWidth)
            {
                pos.X = CameraManager.Camera.Pos.X + GameHelper.ScreenWidth - texture.Width*scale / 2;
                orientation = 90;
            }

            if (Position.X < CameraManager.Camera.Pos.X)                          // to the left
            {
                pos.X = CameraManager.Camera.Pos.X + texture.Width * scale / 2;
                orientation = -90;
            }
            if (Position.Y > CameraManager.Camera.Pos.Y + GameHelper.ScreenHeight) // below
            {
                pos.Y = CameraManager.Camera.Pos.Y + GameHelper.ScreenHeight - texture.Height*scale / 2;
                if (orientation == 0)
                {
                    orientation = 180;
                }
                else
                {
                    orientation = orientation > 0 ? 125 : -125;
                }
            }
            if (Position.Y < CameraManager.Camera.Pos.Y)                             // above
            {
                pos.Y = CameraManager.Camera.Pos.Y + texture.Height * scale / 2;
                if (orientation != 0)
                {
                    orientation = orientation > 0 ? 45 : -45;
                }
            }
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            if (pos != Position)
            {
                pos = CameraManager.Camera.Normalize(pos);
                if (isArrow)
                {
                    orientation = (float)CalcHelper.formatAngle(orientation);
                    orientation = (float)CalcHelper.DegreeToRadian(orientation);
                }
                else
                {
                    orientation = 0;
                }
                double distance = CalcHelper.DistanceBetween(CameraManager.Camera.Pos + GameHelper.HalfScreen, Position);
                Color color = Color.White * MathHelper.Clamp((float)(1 - distance / 1000.0), 0.4f, 1.0f);
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(texture, pos, null, color, orientation, origin, scale, SpriteEffects.None, 1.0f);
                ScreenManager.SpriteBatch.End();
            }
            else
            {
                pos += new Vector2(targetTexture.Width * Scale.X / 2, -70);
                pos = CameraManager.Camera.Normalize(pos);
                orientation = isArrow ? (float)CalcHelper.DegreeToRadian(180) : 0;
                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(texture, pos, null, Color.White, orientation, origin, scale, SpriteEffects.None, 1.0f);
                ScreenManager.SpriteBatch.End();
            }
        }

        private bool _readyToRender = false;
        /// <summary>
        /// Is this object ready to render?
        /// </summary>
        public bool ReadyToRender
        {
            get { return _readyToRender; }
            set { _readyToRender = value; }
        }

        private bool _usedInBoundingBoxCheck;
        public bool UsedInBoundingBoxCheck
        {
            get { return _usedInBoundingBoxCheck; }
            set { _usedInBoundingBoxCheck = value; }
        }

        /// <summary>
        /// The bounding box of this object, used for culling.
        /// </summary>
        public virtual Rectangle BoundingBox
        {
            get
            {
                IPoorTexture tex = TextureManager.GetTexture(TextureName);
                if (tex == null)
                {
                    return new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    Texture2D texture = tex.BaseTexture as Texture2D;
                    int textureWidth = (int)(texture.Width * Scale.X);
                    int textureHeight = (int)(texture.Height * Scale.Y);
                    return new Rectangle(
                            (int)Position.X,
                            (int)Position.Y,
                            textureWidth,
                            textureHeight
                        );
                }
            }
        }

        private Vector2 _position = Vector2.Zero;
        /// <summary>
        /// The position of this object in 2D space.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private float _Z = 1;
        /// <summary>
        /// The Z-position of this object in 3D space.
        /// </summary>
        public float Z
        {
            get { return _Z; }
            set { if (value > 0) { _Z = value; } }
        }

        private Vector2 _scale = Vector2.One;
        /// <summary>
        /// Scale of the object.
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        private Quaternion _rotation = Quaternion.Identity;
        /// <summary>
        /// Yaw, pitch and roll of the object.
        /// </summary>
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        /// <summary>
        /// Draw the object
        /// </summary>
        /// <param name="gameTime">Gametime</param>
        /*public void Draw(GameTime gameTime)
        {
            if (this is IPoorDrawable)
            {
                // Draw
            }
        }*/
    }
}
