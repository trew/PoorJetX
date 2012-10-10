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
        private Texture2D texHealth;
        private Rectangle hpRectOutline;
        private Rectangle healthMeterRect;

        private int smokeTimer;
        private int smokeTimerStartVal;

        private int maxHealth;
        private int health;

        private bool ItsCrashTime;

        public EnemyAirplane(int startHealth):
            base("apTex1")
        {
            Z = 1;
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

            smokeTimer = 1;
            smokeTimerStartVal = 20;

            health = maxHealth = startHealth;
            ItsCrashTime = false; 

            hpRectOutline = new Rectangle(9999,9999, 40, 5);
            healthMeterRect = new Rectangle(9999, 9999, 38, 3);

            texBlack = new Texture2D(EngineManager.Device, 1, 1);
            texBlack.SetData(new Color[] { Color.Black });

            texHealth = new Texture2D(EngineManager.Device, 1, 1);
        }
        
        public override Rectangle BoundingBox
        {
            get
            {
                Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
                return new Rectangle(
                        (int)Position.X - texture.Width / 2,
                        (int)Position.Y - texture.Height / 2,
                        texture.Width,
                        texture.Height
                    );
            }
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
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 origin = new Vector2(texture.Width/2, texture.Height/2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           Position - CameraManager.Camera.Pos, null, Color.AliceBlue,
                                           (float)DegreeToRadian(orientation - 90),
                                           origin, Scale, SpriteEffects.None, 0f);
            
            // Draw health-bar, if plane still alive
            if (!ItsCrashTime)
            {
                ScreenManager.SpriteBatch.Draw(texBlack, Position - CameraManager.Camera.Pos + new Vector2(-10, 20), hpRectOutline, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.Draw(texHealth, Position - CameraManager.Camera.Pos + new Vector2(-9, 21), healthMeterRect, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
            
            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            // Update Healthbar draw-settings.
            healthMeterRect.Width = (int)(38 * ((float)health / maxHealth));
            float hpPercent = ((float)health / maxHealth);
            int red = (int)(255 - 255 * hpPercent);
            int green = (int)(255 * hpPercent);
            texHealth.SetData(new Color[] { new Color(red*3, green*2, 0) });

            
            if (hpPercent < 0.7)
            {
                smokeTimerStartVal = Math.Max(5, (int)(50 * hpPercent));

                smokeTimer--;
                if (smokeTimer <= 0)
                {
                    smokeTimer = smokeTimerStartVal;
                    SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, 0f, new Vector2(0.5f, 0.5f), 200, 15, false, 0.9f));
                }
            }

            if (ItsCrashTime)
            {
                orientation = Math.Min(150, orientation + 0.3);
                airSpeed *= 1.04;
            }

            orientation = formatAngle(orientation);
            velocityAngle = formatAngle(velocityAngle);

            double velocityAngleRotationSpeed = Math.Max(airSpeed, gravity) / (weight * 3);
            double diff = orientation - velocityAngle + 180;
            double posDiff = Math.Abs(diff - 180);

            // Trying to make the enemy stick to a fixed position..
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

        public void Explode()
        {
            SceneGraphManager.RemoveObject(this);
            UsedInBoundingBoxCheck = false;

            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(1, 2), 255, 5, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(3, 2), 255, 6, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 8, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 10, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(1, 2), 255, 5, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(3, 2), 255, 6, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 8, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 10, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(1, 2), 255, 5, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(3, 2), 255, 6, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 8, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 10, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(1, 2), 255, 5, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(3, 2), 255, 6, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 8, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 10, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(1, 2), 255, 5, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(3, 2), 255, 6, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 8, false, 0.5f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, new Vector2(2, 3), 255, 10, false, 0.5f));
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (!ItsCrashTime && health > 0 && collidingWith.GetType() == typeof(Projectile))
            {
                Projectile p = (Projectile)collidingWith;
                health -= p.Damage;
                if (health <= 0)
                {
                    health = 0;
                    ItsCrashTime = true;
                    EngineManager.Score += 1;
                }
            } else if (collidingWith.GetType() == typeof(EnemyAirplane))
            {
                EnemyAirplane e = (EnemyAirplane)(collidingWith);

                // Is the other plane crashing?
                if (e.ItsCrashTime)
                {
                    EngineManager.Score += 2;
                }
                health = 0;
                Explode();
            }
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
