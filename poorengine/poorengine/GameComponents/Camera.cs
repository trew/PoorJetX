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
            maxMoveSpeed = new Vector2(5.5f, 0f);
        }

        public Vector2 Normalize(Vector2 pos)
        {
            return pos - _pos;
        }

        public void Reset()
        {
            moveSpeed = new Vector2(0f, 0f);
        }

        public void changePos(Vector2 change)
        {
            _pos += change;
        }

        public Vector2 SpeedDifference(PlayerAirplane p1)
        {
            return p1.getVelocity() - moveSpeed;
        }

        public void Update(PlayerAirplane player1)
        {
            UpdateX(player1);
            UpdateY(player1);

            changePos(moveSpeed);
            if (Pos.Y > 0)
                Pos = new Vector2(Pos.X, 0);
        }

        public void UpdateX(PlayerAirplane player1)
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

        public void UpdateY(PlayerAirplane player1)
        {
            float screenHeight = EngineManager.Device.Viewport.Height;
            int borderSize = (int)(0.25 * screenHeight);

            // if player is on top of the screen and not facing upwards

            if (player1.getPosition().Y <= Pos.Y + borderSize)
            {
                MoveUp(player1);
            }
            else if (player1.getPosition().Y > Pos.Y + (borderSize))
            {
                MoveDown(player1);
            }
            else
            {
                moveSpeed.Y = 0.0f;
                Pos = new Vector2(Pos.X, 0.0f);
            }

            return;

        }

        public void MoveUp(PlayerAirplane p1)
        {
            if (p1.getVelocity().Y < moveSpeed.Y)
            {
                moveSpeed.Y -= 0.7f;
                moveSpeed.Y = Math.Max(moveSpeed.Y, p1.getVelocity().Y);
            }
        }

        public void MoveDown(PlayerAirplane p1)
        {
            if (p1.getVelocity().Y > moveSpeed.Y)
            {
                moveSpeed.Y += 0.7f;
                moveSpeed.Y = Math.Min(moveSpeed.Y, p1.getVelocity().Y);
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

        public void MoveLeft(PlayerAirplane p1)
        {
            moveSpeed.X -= Math.Max(0.5f, Math.Abs(SpeedDifference(p1).X * 0.13f));
            moveSpeed.X = Math.Max(moveSpeed.X, p1.getVelocity().X);
        }

        public void MoveRight(PlayerAirplane p1)
        {

            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X += 0.07f;
                moveSpeed.X = Math.Min(moveSpeed.X, p1.getVelocity().X);
            }
            moveSpeed.X = Math.Min(moveSpeed.X, maxMoveSpeed.X);
        }

        public void MoveRightMax(PlayerAirplane p1)
        {

            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X += 0.01f;
                moveSpeed.X = Math.Min(moveSpeed.X, p1.getVelocity().X);
            }
        }

        public void MoveRightMegaMax(PlayerAirplane p1)
        {
            if (p1.getVelocity().X > moveSpeed.X)
            {
                moveSpeed.X = Math.Max(moveSpeed.X, p1.getVelocity().X);
            }
        }

        public void SlowDownX(PlayerAirplane p1)
        {
            if (moveSpeed.X > 0) {
                moveSpeed.X -= Math.Max(0.03f, Math.Abs(SpeedDifference(p1).X * 0.05f));
            }
            else if (moveSpeed.X < 0) {
                moveSpeed.X += Math.Max(0.03f, Math.Abs(SpeedDifference(p1).X * 0.05f));
            }
        }

        public void AdjustXMovespeed(PlayerAirplane p1)
        {
            if (moveSpeed.X > p1.getVelocity().X) {
                moveSpeed.X -= 0.01f;
            }
            else if (moveSpeed.X < p1.getVelocity().X) { moveSpeed.X += 0.01f; }
            moveSpeed.X = Math.Min(moveSpeed.X, maxMoveSpeed.X);
        }

        public void AdjustYMovespeed(PlayerAirplane p1)
        {
            if (moveSpeed.Y > p1.getVelocity().Y) { moveSpeed.Y -= 0.03f; }
            else if (moveSpeed.Y < p1.getVelocity().Y) { moveSpeed.Y += 0.08f; }
            moveSpeed.Y = Math.Min(moveSpeed.Y, maxMoveSpeed.Y);
        }
    }
}
