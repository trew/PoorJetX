using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.GameComponents;

namespace PoorEngine.SceneObject
{
    public class Airplane : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        const string airplaneTexture = "apTex1";

        public double thrust;
        public double airSpeed;
        public Vector2 oldPos;
        public double orientation;
        public double lforce;
        public double rforce;
        public double lift;
        public double gravity;
        public Vector2 velocity;
        public double velocityAngle;
        public double linearVelocity;
        public double airDensity;
        public double weight;
        public double angleOfAttack;
        public double angleSpeedModifier;
        
        public Airplane()
        {
            thrust = 0;
            lift = 0;
            orientation = 90;
            airSpeed = 0;
            gravity = 3;
            linearVelocity = 0;
            airDensity = 1.0;
            velocityAngle = 90;
            weight = 1;
            Position = new Vector2(200,200);

        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(airplaneTexture).BaseTexture as Texture2D;
            Vector2 origin;
            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           Position, null, Color.AliceBlue, 
                                           (float)DegreeToRadian(orientation-90),
                                           origin, Scale, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            orientation = formatAngle(orientation);
            velocityAngle = formatAngle(velocityAngle);

            double velocityAngleRotationSpeed = Math.Max(airSpeed, gravity) / (weight * 3);
            double diff = orientation - velocityAngle + 180;
            double posDiff = Math.Abs(diff - 180);

            /*
             * Adjust velocityAngle towards the airplane orientation 
             */
            if (posDiff < velocityAngleRotationSpeed)
            {
                velocityAngle = orientation;
            }
            else if (diff >= 0 && diff < 180 || diff > 359)
            {
                velocityAngle -= velocityAngleRotationSpeed;
            }
            else if (diff >= 180 || diff < 0)
            {
                velocityAngle += velocityAngleRotationSpeed;
            }

            /*
             *  Calculate angle of attack (alpha)
             */
            angleOfAttack = velocityAngle - orientation;
            if (angleOfAttack < -180)
            {
                angleOfAttack = angleOfAttack + 360;
            }
            else if (angleOfAttack > 180)
            {
                angleOfAttack = (360 - angleOfAttack) * -1;
            }

            /*
             *  Calculate movement-direction (X/Y-movement-ratio)
             */
            double newX = Math.Sin(DegreeToRadian(velocityAngle));
            double newY = -Math.Cos(DegreeToRadian(velocityAngle));

            // Save old pos, used for speedcalculations
            oldPos = Position;

            // Calculate lift and drag
            lift = (double)Math.Min(3, Math.Pow(Math.Sqrt(Math.Abs(velocity.X)), 1.45));
            lift = Math.Min(lift, 0.8);

            double drag = Math.Sqrt(Math.Abs(angleOfAttack) / 90);

            // Change rotationspeed of the airplane depending on Angle of Attack
            // WTF NOT USED?
            angleSpeedModifier = Math.Abs(180 - velocityAngle);
            angleSpeedModifier = Math.Sqrt(angleSpeedModifier / 90);


            // Adds 'acceleration'
            if (airSpeed < thrust)
            {
                airSpeed += 0.025;
            }
            else if (airSpeed > thrust)
            {
                airSpeed -= 0.025;
            }
            else // not needed?
            {
                airSpeed = thrust;
            }


            double movementMultiplier = airSpeed - drag;
            
            float xshit = (float)(newX * movementMultiplier );
            float yshit = (float)(((newY * movementMultiplier)) + gravity - lift);

            Position += new Vector2(xshit, yshit);
            velocity = Position - oldPos;

            linearVelocity = Math.Sqrt(
                Math.Pow((Math.Max(Position.X, oldPos.X) - Math.Min(Position.X, oldPos.X)), 2) +
                Math.Pow((Math.Max(Position.Y, oldPos.Y) - Math.Min(Position.Y, oldPos.Y)), 2));

        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/flygplan"), airplaneTexture);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(airplaneTexture);
        }

        /*
         *  Movement
         */
        public void rotateLeft()
        {

        }

        public void rotateRight()
        {
        }

        public void increaseThrust()
        {
        }

        public void decreaseThrust()
        {
        }

        public void killEngine()
        {

        }

        public void HandleInput(Input input)
        {
            double force = linearVelocity;
            double forceIncreaseAmount = force / 20;
            double maxThrust = 7;
            double maxForce = force / 2.7;
            double forceResetAmount = 0.085;

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                orientation -= lforce;
                if (lforce < maxForce)
                {
                    lforce += forceIncreaseAmount;
                }
                else
                {
                    lforce = maxForce;
                }
            }
            else
            {
                if (lforce > 0.005)
                {
                    orientation -= lforce;
                    lforce -= forceResetAmount;
                }
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                orientation += rforce;
                if (rforce < maxForce)
                {
                    rforce += forceIncreaseAmount;
                }
                else
                {
                    rforce = maxForce;
                }
            }
            else
            {
                if (rforce > 0.005)
                {
                    orientation += rforce;
                    rforce -= forceResetAmount;
                }
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                orientation -= 4;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                thrust = 0;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
            {
                if (thrust < maxThrust)
                {
                    thrust += 0.05;
                }
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
            {
                if (thrust >= 0.05)
                {
                    thrust -= 0.05;
                }
            }

        }

        


        /*
         *  Div functions
         */
        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double formatAngle(double oldAngle)
        {
            double newAngle;
            newAngle = oldAngle % 360;

            if (newAngle < 0)
                newAngle += 360;

            return newAngle;
        }
    }
}
