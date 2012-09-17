using System;
using System.Collections.Generic;
using System.Text;

namespace PoorEngine.Interfaces
{
    public interface IPoorLoadable : IPoorSceneObject
    {
        void LoadContent();
        void UnloadContent();
    }
}
