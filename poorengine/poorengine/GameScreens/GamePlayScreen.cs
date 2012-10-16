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
        Airplane player1;

        Instrument throttleMeter;
        Instrument airspeedMeter;

        AmmoController ammoController;

        public GamePlayScreen(int level)
        {
            CameraManager.Reset();
            AmmoManager.Reset();
        }

        public int ScreenWidth
        {
            get { return EngineManager.Device.Viewport.Width; }
        }

        public int ScreenHeight
        {
            get { return EngineManager.Device.Viewport.Height; }
        }

        public Airplane Airplane
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


            LevelManager.CurrentLevel.LoadVisuals();
            LevelManager.CurrentLevel.QueueEnemies();

            ammoController = new AmmoController();
            SceneGraphManager.AddObject(ammoController);

            SkyGradient skyGradient = new SkyGradient("skygradient");
            SceneGraphManager.AddObject(skyGradient);

            player1 = new Airplane();
            SceneGraphManager.AddObject(player1);     

            throttleMeter = new Instrument("instrument", new Vector2(500, ScreenHeight), 0f, 7.5f, 1f, "throttle", this);
            SceneGraphManager.AddObject(throttleMeter);

            airspeedMeter = new Instrument("instrument", new Vector2(800, ScreenHeight), 0f, 13f, 0.5f, "linearvelocity", this);
            SceneGraphManager.AddObject(airspeedMeter);


            // DEBUG =======================
            //SceneGraphManager.AddObject(new AnimatedSprite("anim_smoke1",new Point(100,100), new Point(10,1), Airplane.getPosition(), new Vector2(1,1), 20, false));
            // =============================

            SceneGraphManager.LoadContent();
            ParticleManager.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            SceneGraphManager.Root.Nodes.Clear();
            ParticleManager.UnloadContent();
        }
 

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
 	        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (ScreenState != ScreenState.Active) { return; }

            AmmoManager.Update(gameTime);
            CameraManager.Camera.Update(player1);
            SceneGraphManager.Update(gameTime);
            ParticleManager.Update(gameTime);

            if (player1.IsCrashing)
            {
                ammoController.ReadyToRender = false;
            }

            if (player1.IsDead)
            {
                ExitScreen();
                ScreenManager.AddScreen(new ScoreScreen(EngineManager.Score));
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
                ExitScreen();
                ScreenManager.AddScreen(new ScoreScreen(EngineManager.Score));
            }
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
