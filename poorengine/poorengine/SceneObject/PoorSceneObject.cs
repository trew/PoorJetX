using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Interfaces;

namespace PoorEngine.SceneObject
{
    public class PoorSceneObject : IPoorSceneObject
    {
        private bool _readyToRender = false;
        /// <summary>
        /// Is this object ready to render?
        /// </summary>
        public bool ReadyToRender
        {
            get { return _readyToRender; }
            set { _readyToRender = value; }
        }
        private BoundingBox _boundingBox;
        /// <summary>
        /// The bounding box of this object, used for culling.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
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
        public void Draw(GameTime gameTime)
        {
            if (this is IPoorDrawable)
            {
                // Draw
            }
        }


    }
}
