using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject;
using PoorEngine.Managers;

namespace PoorEngine.GameComponents
{
    public class Camera
    {
        private Vector2 _pos;
        private Vector2 moveSpeed;
        private Vector2 maxMoveSpeed;

        public Camera(Vector2 startPos)
        {
            _pos = startPos;
            moveSpeed = new Vector2(0f, 0f);
            maxMoveSpeed = new Vector2(5.5f, 5.5f);
        }

        public void changePos(Vector2 change)
        {
            _pos += change;
        }

        public void Update(Airplane player1)
        {
            UpdateX(player1);
            UpdateY(player1);

            changePos(moveSpeed);
            if (Pos.Y > 0)
                Pos = new Vector2(Pos.X, 0);
        }

        public void UpdateX(Airplane player1)
        {
            float screenWidth = EngineManager.Device.Viewport.Width;
            int borderSize = (int)(0.156 * screenWidth);

            // Guinness World Record If-statement
            if (player1.Position.X < (Pos.X + borderSize) && !player1.headingRight())
            {
                MoveLeft(player1);
            }

            else if (player1.Position.X < (Pos.X + borderSize) && player1.headingRight())
            {
                SlowDownX(player1);
            }

            else if (player1.Position.X > Pos.X + (screenWidth - borderSize * 0.25f) && player1.headingRight())
            {
                MoveRightMegaMax(player1);
            }

            else if (player1.Position.X > Pos.X + (screenWidth - borderSize * 2f) && player1.headingRight())
            {
                MoveRightMax(player1);
            }

            else if (player1.Position.X > Pos.X + (screenWidth - borderSize * 5f) && player1.headingRight())
            {
                MoveRight(player1);
            }

            else
            {
                AdjustXMovespeed(player1);
            }
        }

        public void UpdateY(Airplane player1)
        {
            float screenHeight = EngineManager.Device.Viewport.Height;
            int borderSize = (int)(0.3 * screenHeight);

            // if player is on top of the screen and not facing upwards
            if (player1.Position.Y < (Pos.Y + borderSize))
            {
                MoveUp(player1);
            }
            else if (player1.Position.Y > Pos.Y + (screenHeight - borderSize * 1.5f))
            {
                MoveDown(player1);
            }

            else if ((player1.Position.Y < (Pos.Y + borderSize)) ||
                     player1.Position.Y > Pos.Y + (screenHeight - borderSize))
            {
                SlowDownY(player1);
            }

                /*

            else if (player1.Position.Y > Pos.Y + (screenHeight - borderSize * 0.25f) && player1.headingUp())
            {
                MoveDownMegaMax(player1);
            }

            else if (player1.Position.Y > Pos.Y + (screenHeight - borderSize * 2f) && player1.headingUp())
            {
                MoveDownMax(player1);
            }
            */

            else
            {
                AdjustYMovespeed(player1);
            }
        }

        public void MoveUp(Airplane p1)
        {
            if (p1.getVelocity().Y < moveSpeed.Y)
            {
                moveSpeed.Y -= 0.2f;
                moveSpeed.Y = Math.Max(moveSpeed.Y, p1.getVelocity().Y);
            }
            moveSpeed.Y = Math.Max(moveSpeed.Y, -maxMoveSpeed.Y);
        }

        public void MoveDown(Airplane p1)
        {

            if (p1.getVelocity().Y > moveSpeed.Y)
            {
                moveSpeed.Y += 0.2f;
                //moveSpeed.Y = Math.Min(moveSpeed.Y, p1.getVelocity().Y);
            }
            moveSpeed.Y = Math.Min(moveSpeed.Y, maxMoveSpeed.Y);
        }

        public void SlowDownY(Airplane p1)
        {
            if (moveSpeed.Y > 0) { moveSpeed.Y -= 0.03f; }
            else if (moveSpeed.Y < 0) { moveSpeed.Y += 0.03f; }
        }


        public void MoveDownMax(Airplane p1)
        {

            if (p1.getVelocity().Y > moveSpeed.Y)
            {
                moveSpeed.Y += 0.01f;
                moveSpeed.Y = Math.Min(moveSpeed.Y, p1.getVelocity().Y);
            }
        }

        public void MoveDownMegaMax(Airplane p1)
        {
            if (p1.getVelocity().Y > moveSpeed.Y)
            {
                moveSpeed.Y = Math.Max(moveSpeed.Y, p1.getVelocity().Y);
            }
        }

        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector2 GetMoveSpeed()
        {
            return moveSpeed;
        }

        public void MoveLeft(Airplane p1)
        {
            if (moveSpeed.X > 1)
            {
                moveSpeed.X -= 0.5f;
            }
            moveSpeed.X -= 0.3f;
            moveSpeed.X = Math.Max(moveSpeed.X, p1.getVelocity().X);
        }

        public void MoveRight(Airplane p1)
        {

            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X += 0.07f;
                moveSpeed.X = Math.Min(moveSpeed.X, p1.getVelocity().X);
            }
            moveSpeed.X = Math.Min(moveSpeed.X, maxMoveSpeed.X);
        }

        public void MoveRightMax(Airplane p1)
        {

            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X += 0.01f;
                moveSpeed.X = Math.Min(moveSpeed.X, p1.getVelocity().X);
            }
        }

        public void MoveRightMegaMax(Airplane p1)
        {
            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X = Math.Max(moveSpeed.X, p1.getVelocity().X);
            }
        }

        public void SlowDownX(Airplane p1)
        {
            if (moveSpeed.X > 0) { moveSpeed.X -= 0.03f; }
            else if (moveSpeed.X < 0) { moveSpeed.X += 0.03f; }
        }

        public void AdjustXMovespeed(Airplane p1)
        {
            if (moveSpeed.X > p1.getVelocity().X) { moveSpeed.X -= 0.01f; }
            else if (moveSpeed.X < p1.getVelocity().X) { moveSpeed.X += 0.01f; }
            moveSpeed.X = Math.Min(moveSpeed.X, maxMoveSpeed.X);
        }

        public void AdjustYMovespeed(Airplane p1)
        {
            if (moveSpeed.Y > p1.getVelocity().Y) { moveSpeed.Y -= 0.03f; }
            else if (moveSpeed.Y < p1.getVelocity().Y) { moveSpeed.Y += 0.03f; }
            moveSpeed.Y = Math.Min(moveSpeed.Y, maxMoveSpeed.Y);
        }
    }
}
