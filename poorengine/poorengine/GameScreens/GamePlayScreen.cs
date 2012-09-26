using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.GameComponents;
using PoorEngine.Textures;
using PoorEngine.SceneObject.SceneGraph;
using PoorEngine.SceneObject;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoorEngine.GameScreens;
using PoorJetX.GameScreens;

namespace PoorEngine.GameScreens
{
    public class GamePlayScreen : GameScreen 
    {
        const string airplaneTexture = "apTex1";
        Airplane player1;
        Background backgroundLayer1;

        public GamePlayScreen()
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
            player1 = new Airplane();
            backgroundLayer1 = new Background("layer-1", 1.0f);

            SceneGraphManager.AddObject(backgroundLayer1);
            SceneGraphManager.AddObject(player1);
            SceneGraphManager.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            SceneGraphManager.Root.Nodes.Clear();
        }
 

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public override void  Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
 	        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            updateCamera();
            SceneGraphManager.Update(gameTime);
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
                UnloadContent();

                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
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
            float screenWidth = EngineManager.Device.Viewport.Width;
            int borderSize = 200;
            float xchange = borderSize - (player1.Position.X - EngineManager.cam.Pos.X);

            if (player1.Position.X < (EngineManager.cam.Pos.X + borderSize))
            {
                EngineManager.cam.changePos(new Vector2(-xchange, 0f));
            }
            else if (player1.Position.X > EngineManager.cam.Pos.X + (screenWidth - borderSize))
            {
                Vector2 changeModifier = new Vector2(player1.Position.X - (EngineManager.cam.Pos.X + screenWidth - borderSize), 0f);
                EngineManager.cam.changePos(changeModifier);
            }
   
        }
    }
}
