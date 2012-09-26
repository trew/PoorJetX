using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.GameComponents
{
    public class Camera
    {
        private Vector2 _pos;

        public Camera(Vector2 startPos)
        {
            _pos = startPos;
        }

        public void changePos(Vector2 change)
        {
            _pos += change;
        }

        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
    }
}
