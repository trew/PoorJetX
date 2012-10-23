using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using PoorEngine.Textures;
using PoorEngine.Helpers;

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
            ScreenManager.SpriteBatch.Begin();
            Text.DrawText(_textFont, _text, Color.Black, _color, outline, 1f, 0f, Position, Vector2.Zero, _centered);
            ScreenManager.SpriteBatch.End();
        }


        // Static functions
        #region Static functions
        public static void DrawTextCentered(SpriteFont font, string text, Color color, float Ypos, float outlineThickness)
        {
            DrawText(font,
                text,
                Color.Black,
                color,
                outlineThickness,
                1f,
                0f,
                new Vector2(0, Ypos),
                Vector2.Zero,
                true);
        }


        public static void DrawText(SpriteFont font, string text, Color color, Vector2 pos, float outlineThickness)
        {
            DrawText(font,
                text,
                Color.Black,
                color,
                outlineThickness,
                1f,
                0f,
                pos,
                Vector2.Zero,
                false);
        }

        /// <summary>
        /// Run this from within SpriteBatch.Begin() and End()
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="backColor"></param>
        /// <param name="frontColor"></param>
        /// <param name="outline"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        /// <param name="centered"></param>
        public static void DrawText(SpriteFont font, string text, Color backColor, Color frontColor, float outline, float scale, float rotation, Vector2 position, Vector2 origin, bool centered)
        {
            float displacement = outline;
            Vector2 textPos;

            if (centered)
            {
                textPos = new Vector2(TextureManager.GetCenterX(0f,
                                                                GameHelper.ScreenWidth,
                                                                font.MeasureString(text).X),
                                      position.Y);
            }
            else
                textPos = position;


            // shadow
            ScreenManager.SpriteBatch.DrawString(font, text, textPos + new Vector2(displacement * scale, displacement * scale),//Here’s the displacement
                backColor,
                rotation,
                origin,
                scale,
                SpriteEffects.None, 1f);

            //These 4 draws are the background of the text and each of them have a certain displacement each way.
            ScreenManager.SpriteBatch.DrawString(font, text, textPos + new Vector2(displacement * scale, displacement * scale),//Here’s the displacement
            backColor,
            rotation,
            origin,
            scale,
            SpriteEffects.None, 1f);

            ScreenManager.SpriteBatch.DrawString(font, text, textPos + new Vector2(-displacement * scale, -displacement * scale),
            backColor,
            rotation,
            origin,
            scale,
            SpriteEffects.None, 1f);

            ScreenManager.SpriteBatch.DrawString(font, text, textPos + new Vector2(-displacement * scale, displacement * scale),
            backColor,
            rotation,
            origin,
            scale,
            SpriteEffects.None, 1f);

            ScreenManager.SpriteBatch.DrawString(font, text, textPos + new Vector2(displacement * scale, -displacement * scale),
            backColor,
            rotation,
            origin,
            scale,
            SpriteEffects.None, 1f);

            //This is the top layer which we draw in the middle so it covers all the other texts except that displacement.

            ScreenManager.SpriteBatch.DrawString(font, text, textPos,
            frontColor,
            rotation,
            origin,
            scale,
            SpriteEffects.None, 1f);

        }

        public static string WordWrap(string str, int width)
        {
            char[] splitChars = new char[] { ' ', '-', '\t' };
            string[] words = Explode(str, splitChars);
            
            int curLineLength = 0;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < words.Length; i += 1)
            {
                string word = words[i];
                // If adding the new word to the current line would be too long,
                // then put it on a new line (and split it up if it's too long).
                if (curLineLength + word.Length > width)
                {
                    // Only move down to a new line if we have text on the current line.
                    // Avoids situation where wrapped whitespace causes emptylines in text.
                    if (curLineLength > 0)
                    {
                        strBuilder.Append(Environment.NewLine);
                        curLineLength = 0;
                    }

                    // If the current word is too long to fit on a line even on it's own then
                    // split the word up.
                    while (word.Length > width)
                    {
                        strBuilder.Append(word.Substring(0, width - 1) + "-");
                        word = word.Substring(width - 1);

                        strBuilder.Append(Environment.NewLine);
                    }

                    // Remove leading whitespace from the word so the new line starts flush to the left.
                    word = word.TrimStart();
                }
                strBuilder.Append(word);
                curLineLength += word.Length;
            }

            return strBuilder.ToString();
        }

        public static string[] Explode(string str, char[] splitChars)
        {
            List<string> parts = new List<string>();
            int startIndex = 0;
            while (true)
            {
                int index = str.IndexOfAny(splitChars, startIndex);

                if (index == -1)
                {
                    parts.Add(str.Substring(startIndex));
                    return parts.ToArray();
                }

                string word = str.Substring(startIndex, index - startIndex);
                char nextChar = str.Substring(index, 1)[0];
                // Dashes and the likes should stick to the word occuring before it. Whitespace doesn't have to.
                if (char.IsWhiteSpace(nextChar))
                {
                    parts.Add(word);
                    parts.Add(nextChar.ToString());
                }
                else
                {
                    parts.Add(word + nextChar);
                }

                startIndex = index + 1;
            }
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
            if (_fontName.Equals("cartoon14"))
                _textFont = ScreenManager.Cartoon14;
            if (_fontName.Equals("cartoon14regular"))
                _textFont = ScreenManager.Cartoon14regular;
            if (_fontName.Equals("cartoon18"))
                _textFont = ScreenManager.Cartoon18;
            if (_fontName.Equals("cartoon18regular"))
                _textFont = ScreenManager.Cartoon18regular;
            if (_fontName.Equals("cartoon24"))
                _textFont = ScreenManager.Cartoon24;
            if (_fontName.Equals("cartoon24regular"))
                _textFont = ScreenManager.Cartoon24regular;
        }

        public void UnloadContent()
        {
            _textFont = null;
        }
    }
}
