using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;

namespace PoorEngine.SceneObject
{
    public class Text : PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        private int _Xappear;
        public int Xappear { get { return _Xappear; } }

        private string _text;
        private bool _centered;
        private string _fontName;
        private SpriteFont _textFont;
        private TimeSpan _spawnTime;
        public TimeSpan SpawnTime { set { _spawnTime = value; } }
        private TimeSpan _TTL;
        private Color _color;
        private float outline;

        /// <summary>
        /// Text-object
        /// </summary>
        public Text(string text, string fontName, Color color, Vector2 position, int Xappear, bool center, float outline, float secondsTTL):
            base("")
        {
            this._text = text;
            this.Z = 0.1f;
            this._Xappear = Xappear;
            this._centered = center;
            this._fontName = fontName;
            this._spawnTime = DateTime.Now.TimeOfDay;
            this._TTL = new TimeSpan(0,0,0,0, (int)(secondsTTL * 1000));
            this._color = color;
            Position = position;
            this.outline = outline;
        }

        public void Draw(GameTime gameTime)
        {
                Text.DrawText(ScreenManager.SpriteBatch, _textFont, _text, Color.Black, _color, outline, 1f, 0f, Position, _centered);
        }


        // Static functions
        #region Static functions
        public static void DrawTextCentered(string fontName, string text, Color color, float Ypos, float outlineThickness)
        {
            DrawText(ScreenManager.SpriteBatch,
                EngineManager.Game.Content.Load<SpriteFont>("Fonts/" + fontName),
                text,
                Color.Black,
                color,
                outlineThickness,
                1f,
                0f,
                new Vector2(0, Ypos),
                true);
        }

        public static void DrawText(string fontName, string text, Color color, Vector2 pos, float outlineThickness)
        {
            DrawText(ScreenManager.SpriteBatch,
                EngineManager.Game.Content.Load<SpriteFont>("Fonts/" + fontName),
                text,
                Color.Black,
                color,
                outlineThickness,
                1f,
                0f,
                pos,
                false);
        }

        public static void DrawText(SpriteBatch sb, SpriteFont font, string text, Color backColor, Color frontColor, float outline, float scale, float rotation, Vector2 position, bool centered)
        {
            float displacement = outline;
            ScreenManager.SpriteBatch.Begin();
            Vector2 textPos;

            if (centered)
            {
                textPos = new Vector2(TextureManager.GetCenterX(0f,
                                                                EngineManager.Device.Viewport.Width,
                                                                font.MeasureString(text).X),
                                      position.Y);
            }
            else
                textPos = position;

            //These 4 draws are the background of the text and each of them have a certain displacement each way.
            sb.DrawString(font, text, textPos + new Vector2(displacement * scale, displacement * scale),//Here’s the displacement
            backColor,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None, 1f);

            sb.DrawString(font, text, textPos + new Vector2(-displacement * scale, -displacement * scale),
            backColor,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None, 1f);

            sb.DrawString(font, text, textPos + new Vector2(-displacement * scale, displacement * scale),
            backColor,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None, 1f);

            sb.DrawString(font, text, textPos + new Vector2(displacement * scale, -displacement * scale),
            backColor,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None, 1f);

            //This is the top layer which we draw in the middle so it covers all the other texts except that displacement.

            sb.DrawString(font, text, textPos,
            frontColor,
            rotation,
            Vector2.Zero,
            scale,
            SpriteEffects.None, 1f);

            ScreenManager.SpriteBatch.End();

        }
        #endregion Static functions

        public void Update(GameTime gameTime)
        {
            if (DateTime.Now.TimeOfDay > _spawnTime + _TTL)
            {
                SceneGraphManager.RemoveObject(this);
            }
        }

        public void LoadContent()
        {
            _textFont = EngineManager.Game.Content.Load<SpriteFont>("Fonts/" + _fontName);
        }

        public void UnloadContent()
        {
            _textFont = null;
        }
    }
}
