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

        private int _lives;

        private Dictionary<string, Instrument> _instruments;

        public GamePlayScreen()
        {
            CameraManager.Reset();
            _instruments = new Dictionary<string, Instrument>();
            _deathTimer = new Stopwatch();
            _completedTimer = new Stopwatch();
            EngineManager.Score = 0;
            SoundFxManager.Clear();
            _lives = 3;
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

            player1 = new PlayerAirplane();
            SceneGraphManager.AddObject(player1);
            _ammoDisplay = new AmmoDisplay(EngineManager.Game, (ProjectileWeapon)player1.ProjectileWeapon, (BombWeapon)player1.BombWeapon);
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
            SoundFxLibrary.AddToLibrary("SoundFX/bombdrop", "bombdrop");
            SoundFxLibrary.AddToLibrary("SoundFX/bombwhistle", "bombwhistle");
            SoundFxLibrary.AddToLibrary("SoundFX/hitplane1", "hitplane1");

            // Add instruments
            throttleMeter = new Instrument(EngineManager.Game, "instrument", new Vector2(150, ScreenHeight), 0f, 7.5f, 0.6f, "throttle", "Throttle", this);
            _instruments.Add("throttleMeter", throttleMeter);

            airspeedMeter = new Instrument(EngineManager.Game, "instrument", new Vector2(270, ScreenHeight), 0f, 13f, 0.6f, "linearvelocity", "Airspeed", this);
            _instruments.Add("airspeedMeter", airspeedMeter);

            foreach (Instrument inst in _instruments.Values)
            {
                inst.LoadContent();
            }
            // !Add instruments

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            player1 = new PlayerAirplane();
            SceneGraphManager.AddObject(player1);
            _ammoDisplay = new AmmoDisplay(EngineManager.Game, (ProjectileWeapon)player1.ProjectileWeapon, (BombWeapon)player1.BombWeapon);
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

                if (!_deathTimer.IsRunning) {
                    _deathTimer.Restart();
                }
                else
                {
                    if (_deathTimer.Elapsed > new TimeSpan(0, 0, 5)) {
                        if (_lives - 1 <= 0)
                        {
                            ExitScreen();
                            ScreenManager.AddScreen(new ScoreScreen());
                        }
                        else
                        {
                            _lives--;
                            this.Reset();
                        }
                    }
                }
            }
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
                            ScreenManager.AddScreen(new VictoryScreen());
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

            if (input.IsNewKeyPress(Keys.Escape))
            {
                SoundFxManager.Pause();
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen(400, 300);
                pauseMenuScreen.ExitToMenuEvent += ExitGameEvent;
                pauseMenuScreen.RestartGameEvent += RestartGameEvent;
                ScreenManager.AddScreen(pauseMenuScreen);
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


            foreach (Instrument inst in _instruments.Values)
            {
                inst.Draw(gameTime);
            }
            _ammoDisplay.Draw(gameTime);

            // Draw Score
            ScreenManager.SpriteBatch.Begin();
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

            if ((player1.IsCrashing || player1.IsDead) && (_lives - 1 <= 0))
            {
                Text.DrawTextCentered(
                    ScreenManager.Cartoon24,
                    "GAME OVER",
                    Color.Red,
                    200,
                    1.3f);
            }

            if (LevelManager.CurrentLevel.Completed)
            {
                Text.DrawTextCentered(
                    ScreenManager.Cartoon24,
                    "Level Completed!",
                    Color.White,
                    200,
                    1.3f);
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
