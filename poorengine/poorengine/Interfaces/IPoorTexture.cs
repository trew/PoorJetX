using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.Interfaces
{
    public interface IPoorTexture
    {
        string FileName
        {
            get;
            set;
        }

        Texture BaseTexture
        {
            get;
        }

        bool ReadyToRender
        {
            get;
        }

        void LoadContent();

        void UnloadContent();
    }
}
