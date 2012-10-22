using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Managers;
using PoorEngine.Settings;

namespace PoorEngine.GameComponents
{
    public class DebugPanel : DrawableGameComponent
    {
        SpriteFont debugFont;
        public bool ViewDebug = true;

        Queue<string> strings;

        public DebugPanel(Game game)
            :base(game)
        {
        }

        public override void  Initialize()
        {
 	        base.Initialize();
        }

        protected override void  LoadContent()
        {
 	        base.LoadContent();
            debugFont = Game.Content.Load<SpriteFont>("Fonts/debug");
            strings = new Queue<string>();
        }

        protected override void  UnloadContent()
        {
 	        base.UnloadContent();
        }

        public override void  Update(GameTime gameTime)
        {
 	        base.Update(gameTime);
            if (EngineManager.Input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D))
            {
                ViewDebug = !ViewDebug;
            }

        }

        public void Print(string s)
        {
            if (ViewDebug)
                strings.Enqueue(s);
        }

        public override void  Draw(GameTime gameTime)
        {
 	        base.Draw(gameTime);
            if (!ViewDebug) return;
            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.DrawString(debugFont, "Debug Panel", new Vector2(50, 5), Color.Black);
            int count = 0;
            string s;
            strings.Enqueue("Total GameTime: " + gameTime.TotalGameTime);
            strings.Enqueue("Elapsed GameTime: " + gameTime.ElapsedGameTime);
            strings.Enqueue("Running Slowly: " + gameTime.IsRunningSlowly);
            strings.Enqueue("GC.GetTotalMemory: " + GC.GetTotalMemory(false) / 1024 + " KB");
            strings.Enqueue("Garbage collection every X frame: " + ScreenManager.getJanitorBreakLength());
            strings.Enqueue("========== VOLUME ============");
            strings.Enqueue("Global volume: " + GameSettings.Default.GlobalSoundVolume);
            strings.Enqueue("Sound volume: " + GameSettings.Default.SoundVolume);
            strings.Enqueue("Music volume: " + GameSettings.Default.MusicVolume);
            strings.Enqueue("==============================");
            strings.Enqueue("SoundFXLoaded: " + SoundFxLibrary.SoundEffectsLoaded);
            strings.Enqueue("TexturesLoaded: " + TextureManager.TexturesLoaded);
            strings.Enqueue("Screens loaded: " + string.Join(", ", ScreenManager.TraceScreens().ToArray()));
            while (strings.Count > 0)
            {
                s = strings.Dequeue();
                ScreenManager.SpriteBatch.DrawString(debugFont, s, new Vector2(50, 5+debugFont.LineSpacing * (count + 1)), Color.Black);
                count++;
            }
            strings.Clear();
            ScreenManager.SpriteBatch.End();
        }
    }
}
