using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using PoorEngine.Managers;
using PoorEngine.GameComponents;
using PoorEngine.Textures;
using PoorEngine.SceneObject;
using PoorEngine.SceneObject.SceneGraph;
using PoorEngine.GameScreens;
using PoorJetX.GameScreens;
using PoorEngine.Particles;
using PoorEngine.Helpers;
using PoorEngine.Settings;
using System.Diagnostics;

namespace PoorEngine.GameScreens
{
    public class GamePlayScreen : GameScreen 
    {
        const string airplaneTexture = "airplane_player";
        PlayerAirplane player1;

        Instrument throttleMeter;
        Instrument airspeedMeter;
        AmmoDisplay _ammoDisplay;

        Stopwatch _deathTimer;
        Stopwatch _completedTimer;

        private Dictionary<string, Instrument> _instruments;

        public GamePlayScreen()
        {
            CameraManager.Reset();
            _instruments = new Dictionary<string, Instrument>();
            _deathTimer = new Stopwatch();
            _completedTimer = new Stopwatch();
            EngineManager.Score = 0;
            SoundFxManager.Clear();
        }

        public void Reset()
        {
            CameraManager.Reset();
            _deathTimer = new Stopwatch();
            _completedTimer = new Stopwatch();
            SceneGraphManager.Root.Nodes.Clear();
            SoundFxManager.Clear();

            LevelManager.Load(LevelManager.CurrentLevel.LevelNumber);
            LevelManager.CurrentLevel.Load();

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            player1.Reset();
            SceneGraphManager.AddObject(player1);
        }

        public int ScreenWidth
        {
            get { return EngineManager.Device.Viewport.Width; }
        }

        public int ScreenHeight
        {
            get { return EngineManager.Device.Viewport.Height; }
        }

        public int GroundLevel
        {
            get { return ScreenHeight - 65; }
        }

        public PlayerAirplane Airplane
        {
            get { return player1; }
        }

        public void LoadTextures()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bomb"), "bomb");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bomb2"), "bomb2");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bullet"), "bullet");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/aabullet"), "aabullet");

            TextureManager.AddTexture(new PoorTexture("Textures/Particles/smoke_black"), "smoke_black");
            TextureManager.AddTexture(new PoorTexture("Textures/Particles/smoke_white"), "smoke_white");
            TextureManager.AddTexture(new PoorTexture("Textures/Particles/fire"), "fire");

            TextureManager.AddTexture(new PoorTexture("Textures/UI/bombtargetmarker"), "bombtargetmarker");

            // Other UI
            TextureManager.AddTexture(new PoorTexture("Textures/UI/1up"), "1up");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammobase_icon"), "ammobase_icon");
            TextureManager.AddTexture(new PoorTexture("Textures/arrow"), "arrow");

            // Animations
            TextureManager.AddTexture(new PoorTexture("Textures/Animations/anim_groundcrash"), "anim_groundcrash");
            TextureManager.AddTexture(new PoorTexture("Textures/Animations/anim_smoke1"), "anim_smoke1");
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            LoadTextures();

            // Sounds
            SoundFxLibrary.AddToLibrary("SoundFX/bomb1", "bomb1");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb2", "bomb2");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb3", "bomb3");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb4", "bomb4");
            SoundFxLibrary.AddToLibrary("SoundFX/huge_explosion", "huge_explosion");
            SoundFxLibrary.AddToLibrary("SoundFX/bombdrop", "bombdrop");
            SoundFxLibrary.AddToLibrary("SoundFX/bombwhistle", "bombwhistle");
            SoundFxLibrary.AddToLibrary("SoundFX/hitplane1", "hitplane1");
            SoundFxLibrary.AddToLibrary("SoundFX/hitplane2", "hitplane2");
            SoundFxLibrary.AddToLibrary("SoundFX/refill", "refill");

            player1 = new PlayerAirplane();
            // Add instruments
            throttleMeter = new Instrument("instrument", new Vector2(150, ScreenHeight), 0f, 7.5f, 0.6f, "throttle", "Throttle", player1);
            _instruments.Add("throttleMeter", throttleMeter);

            airspeedMeter = new Instrument("instrument", new Vector2(270, ScreenHeight), 0f, 13f, 0.6f, "linearvelocity", "Airspeed", player1);
            _instruments.Add("airspeedMeter", airspeedMeter);

            foreach (Instrument inst in _instruments.Values)
            {
                inst.LoadContent();
            }
            // !Add instruments

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            SceneGraphManager.AddObject(player1);
            _ammoDisplay = new AmmoDisplay((ProjectileWeapon)player1.ProjectileWeapon, (BombWeapon)player1.BombWeapon);
            _ammoDisplay.LoadContent();

            SceneGraphManager.LoadContent();
            ParticleManager.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            SceneGraphManager.Root.Nodes.Clear();
            ParticleManager.UnloadContent();
            foreach (Instrument inst in _instruments.Values)
            {
                inst.UnloadContent();
            }
            _instruments.Clear();
            _ammoDisplay = null;
        }
 

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
 	        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (ScreenState != ScreenState.Active || otherScreenHasFocus || coveredByOtherScreen) 
            { 
                return; 
            }

            CameraManager.Camera.Update(player1);
            SceneGraphManager.Update(gameTime);
            ParticleManager.Update(gameTime);
            foreach (Instrument inst in _instruments.Values)
            {
                inst.Update(gameTime);
            }
            _ammoDisplay.Update(gameTime);

            if (player1.IsDead)
            {
                LevelManager.CurrentLevel.Completed = false;
                if (!_deathTimer.IsRunning)
                {
                    _deathTimer.Restart();
                }
                else
                {
                    if (_deathTimer.Elapsed > new TimeSpan(0, 0, 5))
                    {
                        if (player1.Lives <= 0)
                        {
                            ExitScreen();
                            ScreenManager.AddScreen(new ScoreScreen());
                        }
                        else
                        {
                            this.Reset();
                        }
                    }
                }
            }
            else if (player1.IsCrashing)
            {
                LevelManager.CurrentLevel.Completed = false;
            }
            else
            {
                LevelManager.CurrentLevel.CheckCompleted();
                if (LevelManager.CurrentLevel.Completed)
                {
                    player1.UsedInBoundingBoxCheck = false;
                    if (!_completedTimer.IsRunning)
                    {
                        _completedTimer.Restart();
                    }
                    else
                    {
                        if (_completedTimer.Elapsed > new TimeSpan(0, 0, 10))
                        {
                            if (LevelManager.HasNextLevel())
                            {
                                BriefingScreen.Load(LevelManager.CurrentLevel.LevelNumber + 1);
                                this.Reset();
                            }
                            else
                            {
                                ExitScreen();
                                if (LevelManager.CurrentLevel.LevelNumber == 0) //tutorial
                                {
                                    ScreenManager.AddScreen(new BackgroundScreen());
                                    ScreenManager.AddScreen(new MainMenuScreen());
                                }
                                else
                                {
                                    ScreenManager.AddScreen(new VictoryScreen());
                                }
                            }
                        }
                    }
                }
            }
        }
         
        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public override void HandleInput(Input input)
        {
            base.HandleInput(input);
            
            player1.HandleInput(input);


            /*
             *  DEBUG INPUT
             */
            #region
            if (input.IsNewKeyPress(Keys.D1))
            {
                SceneGraphManager.SetTimeOfDay24h = 0f;
            }

            if (input.IsNewKeyPress(Keys.D2))
            {
                SceneGraphManager.SetTimeOfDay24h = 8f;
            }
            if (input.IsNewKeyPress(Keys.D3))
            {
                SceneGraphManager.SetTimeOfDay24h = 12f;
            }
            if (input.IsNewKeyPress(Keys.D4))
            {
                SceneGraphManager.SetTimeOfDay24h = 17f;
            }
            if (input.IsNewKeyPress(Keys.D5))
            {
                SceneGraphManager.SetTimeOfDay24h = 20f;
            }

            if (input.IsNewKeyPress(Keys.E))
            {
                GroundTransport gcv = new GroundTransport(3000, false);
                gcv.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 200f, 
                        GameHelper.GroundLevel -38 );
                gcv.LoadContent();

                SceneGraphManager.AddObject(gcv);
            }

            if (input.IsNewKeyPress(Keys.R))
            {
                AntiAirVehicle gbv = new AntiAirVehicle(3000, false);
                gbv.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 200f,
                        GameHelper.GroundLevel - 47);

                gbv.Velocity = new Vector2(0f, 0f);
                gbv.LoadContent();

                SceneGraphManager.AddObject(gbv);
            }

            if (input.IsNewKeyPress(Keys.B))
            {
                BossAntiAir boss = new BossAntiAir(70000, false);
                boss.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 200f,
                        GameHelper.GroundLevel - 170);

                boss.Velocity = new Vector2(0f, 0f);
                boss.LoadContent();

                SceneGraphManager.AddObject(boss);
            }
            if (input.IsNewKeyPress(Keys.N))
            {
                GroundTransport boss = new GroundTransport(40000, false, true);
                boss.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 200f,
                        GameHelper.GroundLevel - 130);

                boss.Velocity = new Vector2(4f, 0f);
                boss.LoadContent();

                SceneGraphManager.AddObject(boss);
            }

            if (input.IsNewKeyPress(Keys.W))
            {
                AmmoBase ab = new AmmoBase();
                ab.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 250f,
                        GameHelper.GroundLevel + 10);
                ab.LoadContent();

                SceneGraphManager.AddObject(ab);
            }

            if (input.IsNewKeyPress(Keys.P)) {
                LevelManager.CurrentLevel.Completed = true;
            }

            if (input.IsNewKeyPress(Keys.F5))
            {
                SceneGraphManager.ToggleCollisionDetection();
            }

            if (input.IsNewKeyPress(Keys.PageUp))
            {
                ScreenManager.lengthenJanitorCoffeBreak();
            }

            if (input.IsNewKeyPress(Keys.PageDown))
            {
                ScreenManager.shortenJanitorCoffeBreak();
            }
            #endregion
            /*
             *  END DEBUG INPUT
             */

            if (input.IsNewKeyPress(Keys.Escape))
            {
                SoundFxManager.Pause();
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen(400, 300);
                pauseMenuScreen.ExitToMenuEvent += ExitGameEvent;
                pauseMenuScreen.RestartGameEvent += RestartGameEvent;
                ScreenManager.AddScreen(pauseMenuScreen);
            }
            if (input.IsNewKeyPress(Keys.V))
            {
                GameSettings.Default.ShowUI = !GameSettings.Default.ShowUI;
            }
        }

        private void ExitGame()
        {
            ExitScreen();
            ScreenManager.AddScreen(new ScoreScreen());
        }

        private void RestartGameEvent(object sender, EventArgs e)
        {
            this.Reset();
        }

        private void ExitGameEvent(object sender, EventArgs e)
        {
            ExitGame();
        }


        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SceneGraphManager.Draw(gameTime);
            ParticleManager.Draw(gameTime);

            // Draw arrows to important stuff outside the screen

            foreach (Instrument inst in _instruments.Values)
            {
                inst.Draw(gameTime);
            }
            _ammoDisplay.Draw(gameTime);

            // Draw Score
            ScreenManager.SpriteBatch.Begin();

            if ((player1.IsCrashing || player1.IsDead) && (player1.Lives <= 0))
            {
                Text.DrawTextCentered(
                    ScreenManager.Cartoon24,
                    "GAME OVER",
                    Color.Red,
                    200,
                    1.3f);
            }

            else if (LevelManager.CurrentLevel.Completed)
            {
                Text.DrawTextCentered(
                    ScreenManager.Cartoon24,
                    "Level Completed!",
                    Color.White,
                    200,
                    1.3f);
            }

            if (GameSettings.Default.ShowUI)
            {

                Text.DrawText(
                            ScreenManager.Cartoon18, // Font
                            "Level: " + LevelManager.CurrentLevel.LevelNumber,   // Text
                            Color.White,    // Inner color
                            new Vector2(GameHelper.ScreenWidth - 200f, 5f),      // Position
                            1.3f);          // Outline thickness
                Text.DrawText(ScreenManager.Cartoon18, // Font
                            "Score: " + EngineManager.Score,   // Text
                            Color.White,    // Inner color
                            new Vector2(GameHelper.ScreenWidth - 200f, 30f),      // Position
                            1.3f);          // Outline thickness

                for (int i = 0; i < player1.Lives; i++)
                {
                    Texture2D lifeTex = TextureManager.GetTexture("1up").BaseTexture as Texture2D;
                    Rectangle pos = new Rectangle(GameHelper.ScreenWidth - 500 + (50 * i), 10, 48, 31);
                    ScreenManager.SpriteBatch.Draw(lifeTex, pos, Color.White);
                }
            }

            ScreenManager.SpriteBatch.End();

        }

        /// <summary>
        /// This is called when the screen should draw after the UI has drawn.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void PostUIDraw(GameTime gameTime)
        {
            base.PostUIDraw(gameTime);
        }

        private void updateCamera()
        {


        }
    }
}
