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
    public class EnemyAirplane : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private double thrust;
        private double airSpeed;
        private Vector2 oldPos;
        private double orientation;
        private double lforce;
        private double rforce;
        private double lift;
        private double gravity;
        private Vector2 velocity;
        private double velocityAngle;
        private double linearVelocity;
        private double weight;
        private double angleOfAttack;
        private double angleSpeedModifier;
        private Vector2 targetPos;
        private double targetX;

        private Texture2D texBlack;
        private Texture2D texGreen;
        private Rectangle hpRectOutline;
        private Rectangle healthMeterRect;

        private int maxHealth;
        private int health;

        public EnemyAirplane(int startHealth):
            base("apTex1")
        {
            thrust = 3;
            lift = 0;
            orientation = 90;
            airSpeed = 3;
            gravity = 3;
            linearVelocity = 0;
            velocityAngle = 90;
            weight = 1;
            Position = new Vector2(1100,200);
            UsedInBoundingBoxCheck = true;

            health = maxHealth = startHealth;
            hpRectOutline = new Rectangle(9999,9999, 40, 5);
            healthMeterRect = new Rectangle(9999, 9999, 38, 3);

            texBlack = new Texture2D(EngineManager.Device, 1, 1);
            texBlack.SetData(new Color[] { Color.Black });

            texGreen = new Texture2D(EngineManager.Device, 1, 1);
            texGreen.SetData(new Color[] { Color.Green });
        }

        public void setTargetPos(Vector2 tp)
        {
            targetPos = tp;
        }

        public double getLinearVelocity()
        {
            return linearVelocity;
        }

        public double getThrottle()
        {
            return thrust;
        }

        public Vector2 getPosition()
        {
            return Position;
        }
    
        public void Draw(GameTime gameTime)
        {
            EngineManager.Debug.Print("Enemy boundingbox: " + BoundingBox.ToString());

            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 origin = new Vector2(texture.Width/2, texture.Height/2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           Position - CameraManager.Camera.Pos, null, Color.AliceBlue,
                                           (float)DegreeToRadian(orientation - 90),
                                           origin, Scale, SpriteEffects.None, 0f);



            ScreenManager.SpriteBatch.Draw(texBlack, Position - CameraManager.Camera.Pos + new Vector2(-10, 20), hpRectOutline, Color.White, 0f, new Vector2(0,0), 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(texGreen, Position - CameraManager.Camera.Pos + new Vector2(-9, 21), healthMeterRect, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);


            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            healthMeterRect.Width = 38 * (health / maxHealth);

            orientation = formatAngle(orientation);
            velocityAngle = formatAngle(velocityAngle);

            double velocityAngleRotationSpeed = Math.Max(airSpeed, gravity) / (weight * 3);
            double diff = orientation - velocityAngle + 180;
            double posDiff = Math.Abs(diff - 180);

            targetX = CameraManager.Camera.Pos.X + (EngineManager.Device.Viewport.Width * 0.8);

            double xdiff = Position.X - targetX;

            thrust = 5 - xdiff / 300;

            
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
            //lift = (double)Math.Min(3, Math.Pow(Math.Sqrt(Math.Abs(velocity.X)), 1.5));
            //lift = Math.Max(lift, 0.8);

            double drag = Math.Sqrt(Math.Abs(angleOfAttack) / 90);

            // Change rotationspeed of the airplane depending on Angle of Attack
            angleSpeedModifier = Math.Abs(180 - velocityAngle);
            angleSpeedModifier = Math.Sqrt(angleSpeedModifier / 90);

            angleSpeedModifier = 1;

            // Adds 'acceleration'
            if (airSpeed < thrust)
            {
                airSpeed += 0.255 / angleSpeedModifier;
            }
            else if (airSpeed > thrust)
            {
                airSpeed -= 0.250 / angleSpeedModifier;
            }

            double movementMultiplier = airSpeed - drag;
            
            float xmod = (float)(newX * movementMultiplier );
            float ymod = (float)(newY * movementMultiplier);

            Position += new Vector2(xmod, ymod);
            velocity = Position - oldPos;

            linearVelocity = Math.Sqrt(
                Math.Pow((Math.Max(Position.X, oldPos.X) - Math.Min(Position.X, oldPos.X)), 2) +
                Math.Pow((Math.Max(Position.Y, oldPos.Y) - Math.Min(Position.Y, oldPos.Y)), 2));

        }

        public void LoadContent()
        {
            

            TextureManager.AddTexture(new PoorTexture("Textures/airplane"), TextureName);
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
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
            double forceIncreaseAmount = linearVelocity / 20;
            double maxThrust = 7;
            double maxForce = linearVelocity / 2.7;
            double forceResetAmount = 0.085;

            if(input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift)) {
                forceIncreaseAmount /= 2;
                maxForce /= 3;
            }

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


        private double getAngle(Vector2 a, Vector2 b)
        {
            double deltax = a.X - b.X;
            double deltay = a.Y - b.Y;

            double angle_rad = Math.Atan2(deltay, deltax);
            return angle_rad * 180.0 / Math.PI;
        }
    }
}
