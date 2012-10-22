using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject;

namespace PoorEngine.Interfaces
{
    public interface IPoorEnemy
    {
        bool IsDead { get; }
        bool RequiredForVictory { get; }
    }
}
