using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;

namespace PoorEngine.Helpers
{
    public static class GameHelper
    {
        public static int ScreenWidth
        {
            get { return EngineManager.Device.Viewport.Width; }
        }

        public static int ScreenHeight
        {
            get { return EngineManager.Device.Viewport.Height; }
        }

        public static Vector2 HalfScreen
        {
            get { return new Vector2(HalfScreenWidth, HalfScreenHeight); }
        }

        public static float HalfScreenWidth
        {
            get { return ScreenWidth / 2f; }
        }

        public static float HalfScreenHeight
        {
            get { return ScreenHeight / 2f; }
        }

        public static int GroundLevel
        {
            get { return ScreenHeight - 70; }
        }
    }
}
