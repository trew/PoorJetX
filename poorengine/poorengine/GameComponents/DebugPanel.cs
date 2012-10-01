using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Managers;

namespace PoorEngine.GameComponents
{
    public class DebugPanel : DrawableGameComponent
    {
        SpriteFont debugFont;
        int count = 0;

        Queue<string> strings;

        public DebugPanel(Game game)
            :base(game)
        {
        }

        public override void  Initialize()
        {
 	        base.Initialize();
            count = 0;
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
        }

        public void Print(string s)
        {
            strings.Enqueue(s);
        }

        public override void  Draw(GameTime gameTime)
        {
 	        base.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.DrawString(debugFont, "Debug Panel", new Vector2(5, 5), Color.Black);
            int count = 0;
            string s;
            while(strings.Count > 0)
            {
                s = strings.Dequeue();
                ScreenManager.SpriteBatch.DrawString(debugFont, s, new Vector2(5, 5+debugFont.LineSpacing * (count + 1)), Color.Black);
                count++;
            }

            ScreenManager.SpriteBatch.End();
        }
    }
}
