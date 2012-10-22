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

        int janitorCoffeeBreak;
        const int tenMinutes = 30;

        Instrument throttleMeter;
        Instrument airspeedMeter;
        AmmoDisplay _ammoDisplay;

        Stopwatch _deathTimer;
        Stopwatch _completedTimer;

        private int _currentLevelNumber;

        private Dictionary<string, Instrument> _instruments;

        public GamePlayScreen(int level)
        {
            janitorCoffeeBreak = 0;
            CameraManager.Reset();
            _instruments = new Dictionary<string, Instrument>();
            _deathTimer = new Stopwatch();
            _completedTimer = new Stopwatch();
            EngineManager.Score = 0;
            _currentLevelNumber = level;
            SoundFxManager.Clear();
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

        public override void LoadContent()
        {
            base.LoadContent();
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bomb"), "bomb");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bomb2"), "bomb2");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/bullet"), "bullet");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/aabullet"), "aabullet");

            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud1"), "cloud1");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud2"), "cloud2");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud3"), "cloud3");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud4"), "cloud4");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud5"), "cloud5");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/cloud6"), "cloud6");

            TextureManager.AddTexture(new PoorTexture("Textures/Particles/smoke_black"), "smoke_black");
            TextureManager.AddTexture(new PoorTexture("Textures/Particles/smoke_white"), "smoke_white");
            TextureManager.AddTexture(new PoorTexture("Textures/Particles/fire"), "fire");

            TextureManager.AddTexture(new PoorTexture("Textures/Objects/hill1"), "hill1");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/hill2"), "hill2");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/hill3"), "hill3");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain1"), "mountain1");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain2"), "mountain2");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain3"), "mountain3");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain4"), "mountain4");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain5"), "mountain5");
            TextureManager.AddTexture(new PoorTexture("Textures/Objects/mountain6"), "mountain6");

            TextureManager.AddTexture(new PoorTexture("Textures/UI/bombtargetmarker"), "bombtargetmarker");

            // For ammo-related UI
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_bombs"), "ammo_bombs");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_bombs_low"), "ammo_bombs_low");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_bombs_none"), "ammo_bombs_none");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_mg"), "ammo_mg");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_mg_low"), "ammo_mg_low");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_mg_none"), "ammo_mg_none");
            TextureManager.AddTexture(new PoorTexture("Textures/UI/ammo_refill"), "ammo_refill");

            // Animations
            TextureManager.AddTexture(new PoorTexture("Textures/Animations/anim_groundcrash"), "anim_groundcrash");
            TextureManager.AddTexture(new PoorTexture("Textures/Animations/anim_smoke1"), "anim_smoke1");
            
            // Sounds
            SoundFxLibrary.AddToLibrary("SoundFX/bomb1", "bomb1");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb2", "bomb2");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb3", "bomb3");
            SoundFxLibrary.AddToLibrary("SoundFX/bombdrop", "bombdrop");
            SoundFxLibrary.AddToLibrary("SoundFX/bombwhistle", "bombwhistle");
            SoundFxLibrary.AddToLibrary("SoundFX/hitplane1", "hitplane1");

            LevelManager.Load(_currentLevelNumber);
            LevelManager.CurrentLevel.Load();

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            player1 = new PlayerAirplane();
            SceneGraphManager.AddObject(player1);     

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

            SceneGraphManager.LoadContent();
            _ammoDisplay = new AmmoDisplay(EngineManager.Game, (ProjectileWeapon)player1.ProjectileWeapon, (BombWeapon)player1.BombWeapon);
            EngineManager.Game.Components.Add(_ammoDisplay);
            ParticleManager.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            TextureManager.RemoveTexture("bomb");
            TextureManager.RemoveTexture("bomb2");
            TextureManager.RemoveTexture("bullet");
            TextureManager.RemoveTexture("aabullet");

            TextureManager.RemoveTexture("cloud1");
            TextureManager.RemoveTexture("cloud2");
            TextureManager.RemoveTexture("cloud3");
            TextureManager.RemoveTexture("cloud4");
            TextureManager.RemoveTexture("cloud5");
            TextureManager.RemoveTexture("cloud6");

            TextureManager.RemoveTexture("smoke_black");
            TextureManager.RemoveTexture("smoke_white");
            TextureManager.RemoveTexture("fire");

            TextureManager.RemoveTexture("hill1");
            TextureManager.RemoveTexture("hill2");
            TextureManager.RemoveTexture("hill3");
            TextureManager.RemoveTexture("mountain1");
            TextureManager.RemoveTexture("mountain2");
            TextureManager.RemoveTexture("mountain3");
            TextureManager.RemoveTexture("mountain4");
            TextureManager.RemoveTexture("mountain5");
            TextureManager.RemoveTexture("mountain6");

            TextureManager.RemoveTexture("bombtargetmarker");

            // For ammo-related UI
            TextureManager.RemoveTexture("ammo_bombs");
            TextureManager.RemoveTexture("ammo_bombs_low");
            TextureManager.RemoveTexture("ammo_bombs_none");
            TextureManager.RemoveTexture("ammo_mg");
            TextureManager.RemoveTexture("ammo_mg_low");
            TextureManager.RemoveTexture("ammo_mg_none");
            TextureManager.RemoveTexture("ammo_refill");

            // Animations
            TextureManager.RemoveTexture("anim_groundcrash");
            TextureManager.RemoveTexture("anim_smoke1");

            SceneGraphManager.Root.Nodes.Clear();
            ParticleManager.UnloadContent();
            foreach (Instrument inst in _instruments.Values)
            {
                inst.UnloadContent();
            }
            _instruments.Clear();
            EngineManager.Game.Components.Remove(_ammoDisplay);
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
            
            if(janitorCoffeeBreak++ > tenMinutes)
                GC.Collect();

            EngineManager.Debug.Print("Total GameTime: " + gameTime.TotalGameTime);
            EngineManager.Debug.Print("Elapsed GameTime: " + gameTime.ElapsedGameTime);
            EngineManager.Debug.Print("Running Slowly: " + gameTime.IsRunningSlowly);
            EngineManager.Debug.Print("GC.GetTotalMemory: " + GC.GetTotalMemory(false)/1024 + " KB");
            EngineManager.Debug.Print("==============================");
            

            //AmmoManager.Update(gameTime);
            CameraManager.Camera.Update(player1);
            SceneGraphManager.Update(gameTime);
            ParticleManager.Update(gameTime);
            foreach (Instrument inst in _instruments.Values)
            {
                inst.Update(gameTime);
            }

            if (player1.IsDead)
            {

                if (!_deathTimer.IsRunning) {
                    _deathTimer.Restart();
                }
                else
                {
                    if (_deathTimer.Elapsed > new TimeSpan(0, 0, 5)) {
                        ExitScreen();
                        ScreenManager.AddScreen(new GamePlayScreen(LevelManager.CurrentLevel.LevelNumber + 1));
                    }
                }
            }
            LevelManager.CurrentLevel.CheckCompleted();
            if (LevelManager.CurrentLevel.Completed)
            {
                if (!_completedTimer.IsRunning)
                {
                    _completedTimer.Restart();
                }
                else
                {
                    if (_completedTimer.Elapsed > new TimeSpan(0, 0, 10))
                    {
                        ExitScreen();
                        ScreenManager.AddScreen(new GamePlayScreen(LevelManager.CurrentLevel.LevelNumber + 1));
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
                GroundTransport gcv = new GroundTransport(3000);
                gcv.Position = new Vector2(
                        CameraManager.Camera.Pos.X +
                        GameHelper.ScreenWidth - 200f, 
                        GameHelper.GroundLevel -38 );
                gcv.LoadContent();

                SceneGraphManager.AddObject(gcv);
            }

            if (input.IsNewKeyPress(Keys.R))
            {
                AntiAirVehicle gbv = new AntiAirVehicle(3000);
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
            ScreenManager.AddScreen(new ScoreScreen(EngineManager.Score));
        }

        private void RestartGameEvent(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new GamePlayScreen(LevelManager.CurrentLevel.LevelNumber));
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

            // Draw Score
            Text.DrawText("cartoon18", // Font
                        "Level: " + LevelManager.CurrentLevel.LevelNumber,   // Text
                        Color.White,    // Inner color
                        new Vector2(GameHelper.ScreenWidth - 200f, 5f),      // Position
                        1.3f);          // Outline thickness
            Text.DrawText("cartoon18", // Font
                        "Score: " + EngineManager.Score,   // Text
                        Color.White,    // Inner color
                        new Vector2(GameHelper.ScreenWidth - 200f, 30f),      // Position
                        1.3f);          // Outline thickness

            foreach (Instrument inst in _instruments.Values)
            {
                inst.Draw(gameTime);
            }

            if (player1.IsCrashing || player1.IsDead)
            {
                Text.DrawTextCentered("message",
                    "GAME OVER",
                    Color.Red,
                    200,
                    1.3f);
            }

            if (LevelManager.CurrentLevel.Completed)
            {
                Text.DrawTextCentered("message",
                    "Level Completed!",
                    Color.White,
                    200,
                    1.3f);
            }

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
