using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PoorEngine.Interfaces
{
    public interface IPoorShader
    {
        Effect BaseEffect
        {
            get;
        }

        bool ReadyToRender
        {
            get;
        }

        void Initialize(GraphicsDevice graphicsDevice);
    }
}
