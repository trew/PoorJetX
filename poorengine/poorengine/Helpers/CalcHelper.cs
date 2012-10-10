using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.Helpers
{
    public static class CalcHelper
    {
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
    }
}
