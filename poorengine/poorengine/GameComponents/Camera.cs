using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject;

namespace PoorEngine.GameComponents
{
    public class Camera
    {
        private Vector2 _pos;
        private float moveSpeed;

        public Camera(Vector2 startPos)
        {
            _pos = startPos;
            moveSpeed = 0f;
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

        public void MoveLeft(Airplane p1)
        {
            moveSpeed -= 0.3f;
            moveSpeed = Math.Max(moveSpeed, p1.getVelocity().X);
            changePos(new Vector2(moveSpeed, 0f));

            Console.WriteLine(moveSpeed);
        }

        public void MoveRight(Airplane p1)
        {
            moveSpeed += 0.17f;
            moveSpeed = Math.Min(moveSpeed, p1.getVelocity().X);
            changePos(new Vector2(moveSpeed, 0f));

            Console.WriteLine(moveSpeed);
        }

        public void SlowDown(Airplane p1)
        {
            if (moveSpeed > 0) { moveSpeed -= 0.001f; }
            else if (moveSpeed < 0) { moveSpeed += 0.001f; }

            changePos(new Vector2(moveSpeed, 0f));

            Console.WriteLine(moveSpeed);
        }
    }
}
