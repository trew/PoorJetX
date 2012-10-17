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

namespace PoorEngine.GameScreens
{
    public class GamePlayScreen : GameScreen 
    {
        const string airplaneTexture = "apTex1";
        PlayerAirplane player1;

        int janitorCoffeeBreak;
        const int tenMinutes = 30;

        Instrument throttleMeter;
        Instrument airspeedMeter;

        private Dictionary<string, Instrument> _instruments;

        AmmoController ammoController;

        public GamePlayScreen(int level)
        {
            janitorCoffeeBreak = 0;
            CameraManager.Reset();
            AmmoManager.Reset();
            _instruments = new Dictionary<string, Instrument>();
        }

        public int ScreenWidth
        {
            get { return EngineManager.Device.Viewport.Width; }
        }

        public int ScreenHeight
        {
            get { return EngineManager.Device.Viewport.Height; }
        }

        public PlayerAirplane Airplane
        {
            get { return player1; }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            TextureManager.AddTexture(new PoorTexture("Textures/bomb"), "bomb");
            TextureManager.AddTexture(new PoorTexture("Textures/bomb2"), "bomb2");
            TextureManager.AddTexture(new PoorTexture("Textures/bullet"), "bullet");

            TextureManager.AddTexture(new PoorTexture("Textures/cloud1"), "cloud1");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud2"), "cloud2");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud3"), "cloud3");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud4"), "cloud4");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud5"), "cloud5");
            TextureManager.AddTexture(new PoorTexture("Textures/cloud6"), "cloud6");

            TextureManager.AddTexture(new PoorTexture("Textures/hill1"), "hill1");
            TextureManager.AddTexture(new PoorTexture("Textures/hill2"), "hill2");

            TextureManager.AddTexture(new PoorTexture("Textures/emptyshell"), "emptyshell");

            // Animations
            TextureManager.AddTexture(new PoorTexture("Textures/anim_smoke1"), "anim_smoke1");
            TextureManager.AddTexture(new PoorTexture("Textures/anim_explosion1"), "anim_explosion1");
            TextureManager.AddTexture(new PoorTexture("Textures/anim_groundcrash"), "anim_groundcrash");
            
            // Sounds
            SoundFxLibrary.AddToLibrary("SoundFX/bomb1", "bomb1");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb2", "bomb2");
            SoundFxLibrary.AddToLibrary("SoundFX/bomb3", "bomb3");
            SoundFxLibrary.AddToLibrary("SoundFX/bombdrop", "bombdrop");
            SoundFxLibrary.AddToLibrary("SoundFX/bombwhistle", "bombwhistle");
            SoundFxLibrary.AddToLibrary("SoundFX/hitplane1", "hitplane1");

            LevelManager.CurrentLevel.LoadVisuals();
            LevelManager.CurrentLevel.QueueEnemies();
            LevelManager.CurrentLevel.QueueTexts();
            // TODO CurrentLevel.QueueSounds();

            ammoController = new AmmoController();
            SceneGraphManager.AddObject(ammoController);

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            player1 = new PlayerAirplane();
            SceneGraphManager.AddObject(player1);     

            // Add instruments
            throttleMeter = new Instrument(EngineManager.Game, "instrument", new Vector2(500, ScreenHeight), 0f, 7.5f, 1f, "throttle", this);
            _instruments.Add("throttleMeter", throttleMeter);

            airspeedMeter = new Instrument(EngineManager.Game, "instrument", new Vector2(800, ScreenHeight), 0f, 13f, 0.5f, "linearvelocity", this);
            _instruments.Add("airspeedMeter", airspeedMeter);

            foreach (Instrument inst in _instruments.Values)
            {
                inst.LoadContent();
            }
            // !Add instruments


            // DEBUG =======================
            //SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1",new Point(100,100), new Point(10,1), PlayerAirplane.getPosition(), new Vector2(1,1), 20, false));
            // =============================

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
            

            AmmoManager.Update(gameTime);
            CameraManager.Camera.Update(player1);
            SceneGraphManager.Update(gameTime);
            ParticleManager.Update(gameTime);
            foreach (Instrument inst in _instruments.Values)
            {
                inst.Update(gameTime);
            }

            if (player1.IsCrashing)
            {
                ammoController.ReadyToRender = false;
            }

            if (player1.IsDead)
            {
            //    ExitScreen();
            //    ScreenManager.AddScreen(new ScoreScreen(EngineManager.Score));
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

            if (input.IsNewKeyPress(Keys.Escape))
            {
                SoundFxManager.Pause();
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen(400, 300);
                pauseMenuScreen.ExitToMenuEvent += ExitGame;
                ScreenManager.AddScreen(pauseMenuScreen);
            }

        }

        private void ExitGame(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new ScoreScreen(EngineManager.Score));
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

            if (player1.IsCrashing || player1.IsDead)
            {
                Text.DrawTextCentered("message",
                    "GAME OVER",
                    Color.Red,
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
