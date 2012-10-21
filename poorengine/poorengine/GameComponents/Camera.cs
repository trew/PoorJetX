using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject;
using PoorEngine.Managers;
using PoorEngine.Helpers;

namespace PoorEngine.GameComponents
{
    public class Camera
    {
        private Vector2 _pos;
        private Vector2 _velocity;
        private Vector2 _maxVelocity;
        private bool _stopped;

        public Vector2 Velocity { get { return _velocity; } }

        public Camera(Vector2 startPos)
        {
            _pos = startPos;
            _velocity = new Vector2(0f, 0f);
            _maxVelocity = new Vector2(5.5f, 0f);
        }

        public Vector2 Normalize(Vector2 pos)
        {
            return pos - _pos;
        }

        public Rectangle Normalize(Rectangle Hitbox)
        {
            return new Rectangle(Hitbox.X - (int)Pos.X, Hitbox.Y - (int)Pos.Y, Hitbox.Width, Hitbox.Height);
        }

        public void Reset()
        {
            _velocity = new Vector2(0f, 0f);
            _stopped = false;
        }

        public void changePos(Vector2 change)
        {
            _pos += change;
        }

        public Vector2 SpeedDifference(PlayerAirplane p1)
        {
            return p1.Velocity - Velocity;
        }

        public void Update(PlayerAirplane player1)
        {
            if (!_stopped)
            {
                UpdateX(player1);
                UpdateY(player1);

                changePos(_velocity);
                if (Pos.Y > 0)
                    Pos = new Vector2(Pos.X, 0);
            }
        }

        public void UpdateX(PlayerAirplane player1)
        {
            int borderSize = (int)(0.156 * GameHelper.ScreenWidth);

            // Guinness World Record If-statement
            if (player1.Position.X < (Pos.X + borderSize) && !player1.HeadingRight)
            {
                MoveLeft(player1);
            }

            else if (player1.Position.X < (Pos.X + borderSize) && player1.HeadingRight)
            {
                SlowDownX(player1);
            }

            else if (player1.Position.X > Pos.X + (GameHelper.ScreenWidth - borderSize * 0.25f) && player1.HeadingRight)
            {
                MoveRightMegaMax(player1);
            }

            else if (player1.Position.X > Pos.X + (GameHelper.ScreenWidth - borderSize * 2f) && player1.HeadingRight)
            {
                MoveRightMax(player1);
            }

            else if (player1.Position.X > Pos.X + (GameHelper.ScreenWidth - borderSize * 5f) && player1.HeadingRight)
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
            int borderSize = (int)(0.25 * GameHelper.ScreenHeight);

            // if player is on top of the screen and not facing upwards

            if (player1.Position.Y <= Pos.Y + borderSize)
            {
                MoveUp(player1);
            }
            else if (player1.Position.Y > Pos.Y + (borderSize))
            {
                MoveDown(player1);
            }
            else
            {
                _velocity.Y = 0.0f;
                Pos = new Vector2(Pos.X, 0.0f);
            }

            return;

        }

        public void MoveUp(PlayerAirplane p1)
        {
            if (p1.Velocity.Y < _velocity.Y)
            {
                _velocity.Y -= 0.7f;
                _velocity.Y = Math.Max(_velocity.Y, p1.Velocity.Y);
            }
        }

        public void MoveDown(PlayerAirplane p1)
        {
            if (p1.Velocity.Y > _velocity.Y)
            {
                _velocity.Y += 0.7f;
                _velocity.Y = Math.Min(_velocity.Y, p1.Velocity.Y);
            }
        }

        public void Stop()
        {
            _stopped = true;
        }

        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector2 GetMoveSpeed()
        {
            return _velocity;
        }

        public void MoveLeft(PlayerAirplane p1)
        {
            _velocity.X -= Math.Max(0.5f, Math.Abs(SpeedDifference(p1).X * 0.13f));
            _velocity.X = Math.Max(_velocity.X, p1.Velocity.X);
        }

        public void MoveRight(PlayerAirplane p1)
        {

            if (p1.Velocity.X > _velocity.X)
            {
                _velocity.X += 0.07f;
                _velocity.X = Math.Min(_velocity.X, p1.Velocity.X);
            }
            _velocity.X = Math.Min(_velocity.X, _maxVelocity.X);
        }

        public void MoveRightMax(PlayerAirplane p1)
        {

            if (p1.Velocity.X > _velocity.X)
            {
                _velocity.X += 0.01f;
                _velocity.X = Math.Min(_velocity.X, p1.Velocity.X);
            }
        }

        public void MoveRightMegaMax(PlayerAirplane p1)
        {
            if (p1.Velocity.X > _velocity.X)
            {
                _velocity.X = Math.Max(_velocity.X, p1.Velocity.X);
            }
        }

        public void SlowDownX(PlayerAirplane p1)
        {
            if (_velocity.X > 0) {
                _velocity.X -= Math.Max(0.03f, Math.Abs(SpeedDifference(p1).X * 0.05f));
            }
            else if (_velocity.X < 0) {
                _velocity.X += Math.Max(0.03f, Math.Abs(SpeedDifference(p1).X * 0.05f));
            }
        }

        public void AdjustXMovespeed(PlayerAirplane p1)
        {
            if (_velocity.X > p1.Velocity.X) {
                _velocity.X -= 0.01f;
            }
            else if (_velocity.X < p1.Velocity.X) { _velocity.X += 0.01f; }
            _velocity.X = Math.Min(_velocity.X, _maxVelocity.X);
        }

        public void AdjustYMovespeed(PlayerAirplane p1)
        {
            if (_velocity.Y > p1.Velocity.Y) { _velocity.Y -= 0.03f; }
            else if (_velocity.Y < p1.Velocity.Y) { _velocity.Y += 0.08f; }
            _velocity.Y = Math.Min(_velocity.Y, _maxVelocity.Y);
        }
    }
}
