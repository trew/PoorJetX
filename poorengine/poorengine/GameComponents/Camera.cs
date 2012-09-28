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
        private float maxMoveSpeed;

        public Camera(Vector2 startPos)
        {
            _pos = startPos;
            moveSpeed = 0f;
            maxMoveSpeed = 5.5f;
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

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }

        public void MoveLeft(Airplane p1)
        {
            if (moveSpeed > 1)
            {
                moveSpeed -= 0.5f;
            }
            moveSpeed -= 0.3f;
            moveSpeed = Math.Max(moveSpeed, p1.getVelocity().X);
            changePos(new Vector2(moveSpeed, 0f));
        }

        public void MoveRight(Airplane p1)
        {
            
            if (p1.getVelocity().X > moveSpeed)
            {
                moveSpeed += 0.07f;
                moveSpeed = Math.Min(moveSpeed, p1.getVelocity().X);
            }
            moveSpeed = Math.Min(moveSpeed, maxMoveSpeed);
            changePos(new Vector2(moveSpeed, 0f));
        }

        public void MoveRightMax(Airplane p1)
        {

            if (p1.getVelocity().X > moveSpeed)
            {
                moveSpeed += 0.01f;
                moveSpeed = Math.Min(moveSpeed, p1.getVelocity().X);
            }
            changePos(new Vector2(moveSpeed, 0f));
        }

        public void MoveRightMegaMax(Airplane p1)
        {
            if (p1.getVelocity().X > moveSpeed)
            {
                moveSpeed = Math.Max(moveSpeed, p1.getVelocity().X);
            }
            changePos(new Vector2(moveSpeed, 0f));
        }

        public void SlowDown(Airplane p1)
        {
            if (moveSpeed > 0) { moveSpeed -= 0.03f; }
            else if (moveSpeed < 0) { moveSpeed += 0.03f; }

            changePos(new Vector2(moveSpeed, 0f));
        }

        public void AdjustMovespeed(Airplane p1)
        {
            if (moveSpeed > p1.getVelocity().X) { moveSpeed -= 0.01f; }
            else if (moveSpeed < p1.getVelocity().X) { moveSpeed += 0.01f; }
            moveSpeed = Math.Min(moveSpeed, maxMoveSpeed);
        }

        public void KeepGoing()
        {
            changePos(new Vector2(moveSpeed, 0f));
        }
    }
}
