using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;

namespace PoorEngine.SceneObject
{
    public class PoorSceneObject : IPoorSceneObject
    {
        public PoorSceneObject(String textureName)
        {
            TextureName = textureName;
        }

        public String TextureName { get; set; }

        public virtual void Collide(PoorSceneObject collidingWith) {
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
