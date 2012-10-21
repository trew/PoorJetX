using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;

namespace PoorEngine.Helpers
{
    public static class CalcHelper
    {
        private static Random random = new Random();
        public static Random Random { get { return random; } }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static Vector2 calculatePoint(Vector2 currentPos, float direction, float distance)
        {
            float newX = (float)Math.Sin(DegreeToRadian(direction)) * distance;
            float newY = (float)-Math.Cos(DegreeToRadian(direction)) * distance;

            return currentPos + new Vector2(newX, newY);
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double formatAngle(double oldAngle)
        {
            double newAngle;
            newAngle = oldAngle % 360;

            if (newAngle < 0)
                newAngle += 360;

            return newAngle;
        }

        public static double getAngle(Vector2 a, Vector2 b)
        {
            double deltax = a.X - b.X;
            double deltay = a.Y - b.Y;
            double angle_rad = Math.Atan2(deltay, deltax);

            return angle_rad * 180.0 / Math.PI;
        }

        public static double getAngleAsRadian(Vector2 a, Vector2 b)
        {
            double deltax = a.X - b.X;
            double deltay = a.Y - b.Y;

            return Math.Atan2(deltay, deltax);
        }



        public static double DistanceBetween(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(
                Math.Pow((Math.Max(a.X, b.X) - Math.Min(a.X, b.X)), 2) +
                Math.Pow((Math.Max(a.Y, b.Y) - Math.Min(a.Y, b.Y)), 2));
        }

        public static double DistanceToMiddle(Vector2 objectPos)
        {
            Vector2 middlePos = CameraManager.Camera.Pos + new Vector2(GameHelper.HalfScreenWidth,
                                                                       GameHelper.HalfScreenWidth);

            return DistanceBetween(objectPos, middlePos);
        }

        public static Vector2 CalcPan(Vector2 pos)
        {
            Vector2 relativeToCamera = CameraManager.Camera.Normalize(pos);
            float x = (relativeToCamera.X / GameHelper.ScreenWidth) - 0.5f;
            float y = (relativeToCamera.Y / GameHelper.ScreenHeight) - 0.5f;

            x = MathHelper.Clamp(x, -0.5f, 0.5f);
            y = MathHelper.Clamp(y, -0.5f, 0.5f);

            return new Vector2(x, y);
        }

        public static float CalcVolume(Vector2 position)
        {
            float deadzone = 700f;    // How far from the middle of the screen will volume start going down
            float borderzone = 1500f; // Over how long distance will the volume go from 100% to 0%
            float distToMid = (float)CalcHelper.DistanceToMiddle(position);

            return MathHelper.Clamp((float)(borderzone - (distToMid - deadzone)) / borderzone, 0f, 1f);
        }
    }
}
