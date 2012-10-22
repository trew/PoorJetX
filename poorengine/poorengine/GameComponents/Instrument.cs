using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.GameComponents;
using PoorEngine.GameScreens;
using PoorEngine.Helpers;

namespace PoorEngine.SceneObject
{
    public class Instrument: GameComponent
    {
        private bool _visible;
        public bool Visible { get { return _visible; } set { _visible = value; } }
        float currentValue;
        float minValue;
        float maxValue;
        Vector2 pos;
        float scale;
        Texture2D needleTexture;
        float texHeight;
        private string _sourceName;
        GamePlayScreen currentScreen;
        private string _titleString;
        private string _texName;
        public string TextureName { get { return _texName; } }

        public Instrument(Game game, string texName, Vector2 position, 
            float minVal, float maxVal, float scaleVal, 
            string sourceName, string titleString, GamePlayScreen gameScreen):
            base(game)
        {
            _titleString = titleString;
            _texName = texName;
            minValue = minVal;
            maxValue = maxVal;
            currentScreen = gameScreen;
            pos = position;
            _sourceName = sourceName;
            scale = scaleVal;
            needleTexture = new Texture2D(EngineManager.Device, 1, 1, false, SurfaceFormat.Color);
            needleTexture.SetData(new[] { Color.White });
        }



        void DrawLine(SpriteBatch batch, Texture2D texture,
             float width, float length, float angle, Color color, Vector2 startPoint)
        {
            batch.Draw(texture,
                startPoint,
                null,
                color,
                (float)CalcHelper.DegreeToRadian(angle - 90),
                Vector2.Zero,
                new Vector2(length, width),
                SpriteEffects.None, 0);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_sourceName == "throttle")
            {
                currentValue = (float)currentScreen.Airplane.Thrust;
            }
            else if (_sourceName == "linearvelocity")
            {
                currentValue = (float)currentScreen.Airplane.LinearVelocity;
            }
            if (EngineManager.Input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.V))
            {
                Visible = !Visible;
            }
            if (currentScreen.Airplane.IsCrashing)
            {
                Visible = false;
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!Visible) return;
            // Standardize input, calculate positions
            float max_sd = maxValue - minValue;
            float curr_sd = currentValue - minValue;
            float offset180multiplier = max_sd / 180;
            float curr_real = curr_sd / offset180multiplier;
         
            Texture2D tex = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

            Vector2 scaledPos = pos - new Vector2(0, tex.Height*scale);
            Vector2 needle_origin = scaledPos + new Vector2(tex.Width * scale / 2, tex.Height * scale);

            ScreenManager.SpriteBatch.Begin();
            // Draw instrument
            ScreenManager.SpriteBatch.Draw(tex, scaledPos, null, Color.White, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            // Draw needle
            DrawLine(ScreenManager.SpriteBatch, needleTexture, 3, (texHeight - 15) * scale, curr_real - 90, Color.White, needle_origin);
           
            ScreenManager.SpriteBatch.End();

            // Draw title-text
            Text.DrawText(
                        ScreenManager.Cartoon14, // Font
                        _titleString,   // Text
                        Color.White,    // Inner color
                        scaledPos + new Vector2(TextureManager.GetCenterX(0f,
                                                                tex.Width * scale,
                                                                ScreenManager.Cartoon14.MeasureString(_titleString).X),-20f),      // Position
                        1.3f);          // Outline thickness

            
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/UI/instrument"), TextureName);

            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            texHeight = texture.Height;
            Visible = true;
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
            Visible = false;
        }
    }
}
