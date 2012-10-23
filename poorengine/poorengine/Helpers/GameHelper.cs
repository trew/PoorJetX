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

        public static Vector2 ScreenMiddle
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

        /// <summary>
        /// Returns an integer telling if outside screen and at what direction. 0 = not outside. 1 = over, 2 = right, 4 = under, 8 left, or an combination of them, 6 = down right.
        /// </summary>
        public static int IsOutOfView(Vector2 position)
        {
            /*  
             *   __1__
             * 8 |____| 2
             *     4
             */ 
            int returnValue = 0;

            if (position.X < CameraManager.Camera.Pos.X) // Left
                returnValue += 8;

            if (position.X > CameraManager.Camera.Pos.X + ScreenWidth) // Right
                returnValue += 2;

            if (position.Y < CameraManager.Camera.Pos.Y) // Up
                returnValue += 1;

            if (position.Y > CameraManager.Camera.Pos.Y + ScreenHeight) // Down
                returnValue += 4;

            return returnValue;
        }

        public static int GroundLevel
        {
            get { return ScreenHeight - 70; }
        }
    }
}
