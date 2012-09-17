using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.Interfaces
{
    public interface IPoorUpdateable : IPoorSceneObject
    {
        void Update(GameTime gameTime);
    }
}
