using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.Interfaces
{
    public interface IPoorWeaponHolder
    {
        float Orientation { get; }
        Vector2 Velocity { get; }
        Vector2 Position { get; }
    }
}
