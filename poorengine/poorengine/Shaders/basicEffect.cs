using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PoorEngine.Shaders
{
    public class basicEffect : IPoorShader
    {
        private BasicEffect _baseEffect;
        /// <summary>
        /// Gets the underlying effect
        /// </summary>
        public Effect BaseEffect
        {
            get { return _baseEffect; }
        }

        private bool _readyToRender;
        /// <summary>
        /// Is the effect ready to be rendered?
        /// </summary>
        public bool ReadyToRender
        {
            get { return _readyToRender; }
        }

        /// <summary>
        /// Initializes the effect from byte code for the given GraphicsDevice
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice for which the effect is being created.</param>
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _baseEffect = new BasicEffect(graphicsDevice);
            _readyToRender = true;
        }
    }
}
