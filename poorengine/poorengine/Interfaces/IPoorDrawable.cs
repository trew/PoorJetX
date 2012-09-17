using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.Interfaces
{
    public interface IPoorDrawable : IPoorSceneObject
    {
        void Draw(GameTime gameTime);
    }
}
