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
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class Airplane : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
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
        public bool IsDead { get; set; }
        private int health;
        private int maxHealth;
        public bool IsCrashing { get; set; }

//        private Texture2D texBlack;
//        private Texture2D texHealth;
        private Rectangle hpRectOutline;
        private Rectangle healthMeterRect;

        private int smokeTimer;
        private int smokeTimerStartVal;

        // Sounds
        int engineFX_id;
        int diveFX_id;
        int fireBulletFX_id;
        const float SOUNDVOLUME = 0.6f;

        public Airplane():
            base("apTex1")
        {
            thrust = 4;
            lift = 0;
            orientation = 90;
            airSpeed = 4;
            gravity = 3;
            linearVelocity = 0;
            velocityAngle = 90;
            weight = 1;
            Position = new Vector2(200, 500);
            Z = 0.999f;
            UsedInBoundingBoxCheck = true;
            IsDead = false;
            IsCrashing = false;
            health = maxHealth = 2000;

            hpRectOutline = new Rectangle(9999, 9999, 40, 5);
            healthMeterRect = new Rectangle(9999, 9999, 38, 3);

//            texBlack = new Texture2D(EngineManager.Device, 1, 1);
//            texBlack.SetData(new Color[] { Color.Black });

//            texHealth = new Texture2D(EngineManager.Device, 1, 1);

            smokeTimer = 1;
            smokeTimerStartVal = 20;

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


        public double getLinearVelocity()
        {
            return linearVelocity;
        }

        public double getThrottle()
        {
            return thrust;
        }

        public float getOrientation()
        {
            return (float)orientation;
        }

        public Vector2 getPosition()
        {
            return Position;
        }


        public Vector2 getVelocity()
        {
            return velocity;
        }

        public float HitPointsPercent { get { return ((float)health / maxHealth); } }

        public void HandleDebugInput(Input input)
        {
            if (input.IsNewKeyPress(Keys.Y))
            {
                TakeDamage(200);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                orientation -= 4;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                thrust = 0;
            }

            if (input.IsNewKeyPress(Keys.O))
            {
                ParticleManager.Explosion.AddParticles(CameraManager.Camera.Pos + new Vector2(EngineManager.Device.Viewport.Width / 2 + 100,EngineManager.Device.Viewport.Height -30 ));
            }
        }

        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0)
            {
                IsCrashing = true;
                health = 0;
            }
        }

        public override void Collide(PoorSceneObject collidingWith)
        {
            if (IsCrashing) return;

            if (collidingWith.GetType() == typeof(Projectile))
            {
                Projectile proj = (Projectile)collidingWith;
                TakeDamage(proj.Damage);
            }
        }

        public void Draw(GameTime gameTime)
        {
            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(texture,
                                           Position - CameraManager.Camera.Pos, null, Color.AliceBlue,
                                           (float)DegreeToRadian(orientation - 90),
                                           origin, Scale, SpriteEffects.None, 0f);

            // Draw health-bar, if plane still alive
            if (!IsCrashing)
            {
                // Update Healthbar draw-settings.
                healthMeterRect.Width = (int)(38 * ((float)health / maxHealth));
                int red = (int)(255 - 255 * HitPointsPercent);
                int green = (int)(255 * HitPointsPercent);

                Color hpColor = new Color(red * 3, green * 2, 0);

                Texture2D texBlack = TextureManager.GetColorTexture(Color.Black);
                Texture2D texHealth = TextureManager.GetColorTexture(hpColor);
                ScreenManager.SpriteBatch.Draw(texBlack, Position - CameraManager.Camera.Pos + new Vector2(-10, 20), hpRectOutline, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                ScreenManager.SpriteBatch.Draw(texHealth, Position - CameraManager.Camera.Pos + new Vector2(-9, 21), healthMeterRect, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            EngineManager.Device.Textures[0] = null;

            // Spawn smoke-effects if HP is low
            if (HitPointsPercent < 0.7)
            {
                smokeTimerStartVal = Math.Max(5, (int)(50 * HitPointsPercent));

                smokeTimer--;
                if (smokeTimer <= 0)
                {
                    smokeTimer = smokeTimerStartVal;
                    SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position, 0f, new Vector2(0.5f, 0.5f), 200, 15, false, 0.9f));
                }
            }

            // Execute airplane-crash. Damn i love this comment.
            if (IsCrashing)
            {
                orientation = Math.Min(150, orientation + 0.3);
            }

            // Standardize angle-values
            orientation = formatAngle(orientation);
            velocityAngle = formatAngle(velocityAngle);

            // Calculate alpha-diff
            double velocityAngleRotationSpeed = Math.Max(airSpeed, gravity) / (weight * 3);
            double diff = orientation - velocityAngle + 180;
            double posDiff = Math.Abs(diff - 180);

            
            // Adjust velocityAngle towards the airplane orientation 
            
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


            // Calculate angle of attack (alpha)
            angleOfAttack = velocityAngle - orientation;
            if (angleOfAttack < -180)
            {
                angleOfAttack = angleOfAttack + 360;
            }
            else if (angleOfAttack > 180)
            {
                angleOfAttack = (360 - angleOfAttack) * -1;
            }

            
            // Calculate movement-direction (X/Y-movement-ratio)
            double newX = Math.Sin(CalcHelper.DegreeToRadian(velocityAngle));
            double newY = -Math.Cos(CalcHelper.DegreeToRadian(velocityAngle));


            // Save old pos, used for speedcalculations
            oldPos = Position;

            // Calculate lift and drag
            lift = (double)Math.Min(3, Math.Pow(Math.Sqrt(Math.Abs(velocity.X)), 1.5));
            lift = Math.Max(lift, 0.8);

            double drag = Math.Sqrt(Math.Abs(angleOfAttack) / 90);

            // Change rotationspeed of the airplane depending on Angle of Attack
            angleSpeedModifier = Math.Abs(180 - velocityAngle);
            angleSpeedModifier = Math.Sqrt(angleSpeedModifier / 90);

            angleSpeedModifier = 1;

            // Adds 'acceleration'
            if (airSpeed < thrust)
            {
                airSpeed += 0.025 / angleSpeedModifier;
            }
            else if (airSpeed > thrust)
            {
                airSpeed -= 0.010 / angleSpeedModifier;
            }

            double movementMultiplier = airSpeed - drag;

            float xmod = (float)(newX * movementMultiplier);
            float ymod = (float)(((newY * movementMultiplier)) + gravity - lift);
            ymod += (float)(9.8 * gameTime.ElapsedGameTime.TotalSeconds);

            Position += new Vector2(xmod, ymod);
            velocity = Position - oldPos;

            linearVelocity = CalcHelper.DistanceBetween(Position, oldPos);

            if (airSpeed + 0.1 < linearVelocity)
            {
                airSpeed += 0.025 / angleSpeedModifier;
            }

            if (!IsCrashing)
                airSpeed = Math.Min(airSpeed, 8);
            else
                airSpeed = Math.Min(airSpeed, 20);

            // Update soundFX based on airSpeed-calculations etc
            updateSound();
        }

        void updateSound()
        {
            float enginePitch = (float)(Math.Pow((thrust / 9),1.8) - 0.1f);
            enginePitch += (float)(linearVelocity / 15);
            SoundFxManager.GetByID(engineFX_id).Pitch = MathHelper.Clamp(enginePitch, -1f, 1f);
            SoundFxManager.GetByID(engineFX_id).Volume = MathHelper.Clamp(enginePitch, 0.6f, 1f);
            SoundFxManager.GetByID(engineFX_id).Volume *= SOUNDVOLUME;

            // Add dive-sound if speed is high enough
            float airspeedPitch;
            if (linearVelocity > 7)
            {
                airspeedPitch = (float)((linearVelocity -7) / 7);
                airspeedPitch -= 0.1f;
                SoundFxManager.GetByID(diveFX_id).Pitch = MathHelper.Clamp(airspeedPitch, -1f, 1f);
                SoundFxManager.GetByID(diveFX_id).Volume = MathHelper.Clamp(airspeedPitch, 0f, 1f);
                SoundFxManager.GetByID(diveFX_id).Volume *= SOUNDVOLUME;
            }
            else
            {
                SoundFxManager.GetByID(diveFX_id).Volume = 0f;
            }


            SoundFxManager.GetByID(engineFX_id).Pan = CalcHelper.CalcPan(Position).X;
            SoundFxManager.GetByID(diveFX_id).Pan = CalcHelper.CalcPan(Position).X;

            EngineManager.Debug.Print("Relpos X: " + CalcHelper.CalcPan(Position).X);
            EngineManager.Debug.Print("Relpos Y: " + CalcHelper.CalcPan(Position).Y);




            
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/flygplan"), TextureName);

            SoundFxLibrary.AddToLibrary("SoundFX/engine1", "engine1");
            SoundFxLibrary.AddToLibrary("SoundFX/dive1", "dive1");
            SoundFxLibrary.AddToLibrary("SoundFX/firebullet", "firebullet");

            engineFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("engine1"));
            diveFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("dive1"));
            fireBulletFX_id = SoundFxManager.AddInstance(SoundFxLibrary.GenerateInstance("firebullet"));
            SoundFxManager.GetByID(engineFX_id).Volume = 0.3f;

            SoundFxManager.GetByID(engineFX_id).IsLooped = true;
            SoundFxManager.GetByID(engineFX_id).Volume = 0.6f;
            SoundFxManager.GetByID(engineFX_id).Play();

            SoundFxManager.GetByID(diveFX_id).IsLooped = true;
            SoundFxManager.GetByID(diveFX_id).Volume = 0f;
            SoundFxManager.GetByID(diveFX_id).Play();
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

        public bool headingRight()
        {
            return velocity.X > 0;
        }

        public bool headingDown()
        {
            return velocity.Y > 0;
        }

        public void HandleInput(Input input)
        {
            HandleDebugInput(input);

            if (IsCrashing) return;

            double forceIncreaseAmount = MathHelper.Clamp((float)(0.1 / linearVelocity), 0.002f, 0.4f);
            double maxThrust = 7;
            double maxForce = MathHelper.Clamp((float)(3 / linearVelocity), 0.3f, 1.3f);
            double forceResetAmount = 0.085;


            if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                forceIncreaseAmount *= 3;
                maxForce = MathHelper.Clamp((float)(7.5 / linearVelocity), 1.2f, 1.5f);
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (airSpeed > 7.2 && lforce > 0.2)
                {
                    drawWingtipVortices();
                }

                orientation -= lforce;
                lforce = MathHelper.Clamp((float)(lforce + forceIncreaseAmount), 0f, (float)maxForce);
                
            }
            else
            {
                lforce = MathHelper.Clamp((float)(lforce -= forceResetAmount), 0f, 10f);
                orientation -= lforce;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (airSpeed > 7.2 && rforce > 0.2)
                {
                    drawWingtipVortices();
                }

                orientation += rforce;
                rforce = MathHelper.Clamp((float)(rforce + forceIncreaseAmount), 0f, (float)maxForce);
            }
            else
            {
                rforce = MathHelper.Clamp((float)(rforce -= forceResetAmount), 0f, 10f);
                orientation += rforce;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
            {
                if (thrust < maxThrust)
                    thrust += 0.05;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Z))
            {
                if (thrust >= 0.05)
                    thrust -= 0.05;
            }

            if (input.IsNewKeyPress(Keys.Space))
            {
                if (AmmoManager.dropBomb())
                {
                    SceneGraphManager.AddObject(new Projectile(CalcHelper.calculatePoint(Position, (float)orientation + 90, 10f), velocity, "bomb2", 0.13f));
                }
            }

            if (input.LastKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (AmmoManager.fireBullet())
                {
                    float volume = MathHelper.Clamp(CalcHelper.CalcVolume(Position) * 0.3f, 0f, 0.3f);
                    float pan = CalcHelper.CalcPan(Position).X;

                    SoundFxLibrary.GetFx("firebullet").Play(
                                                            volume, 
                                                            CalcHelper.RandomBetween(-0.2f, 0.3f), 
                                                            pan);

                    SceneGraphManager.AddObject(new Projectile(CalcHelper.calculatePoint(Position, (float)orientation, 30f), velocity, 15f, (float)orientation, 3f, "bullet"));
                    ParticleManager.ProjectileHit.AddParticles(AmmoManager.LastBulletPos + CameraManager.Camera.Pos);
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

        private void drawWingtipVortices()
        {
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position + new Vector2(0, -4), (float)DegreeToRadian(orientation), new Vector2(0.1f, 0.8f), 250, 5, false, 0.9f));
            SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1", new Point(100, 100), new Point(10, 1), Position + new Vector2(2, 10), (float)DegreeToRadian(orientation), new Vector2(0.1f, 0.8f), 250, 5, false, 0.9f));
        }
    }
}
