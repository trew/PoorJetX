using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Managers;
using PoorEngine.Textures;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Interfaces;
using PoorEngine.GameScreens;
using PoorEngine.Helpers;
using PoorEngine.SceneObject;
using PoorEngine.Settings;


namespace PoorEngine.GameComponents
{
    public class Instrument
    {
        float currentValue;
        float minValue;
        float maxValue;
        Vector2 pos;
        float scale;
        Texture2D needleTexture;
        float texHeight;
        private string _sourceName;
        PlayerAirplane _player;
        private string _titleString;
        private string _texName;
        public string TextureName { get { return _texName; } }

        public Instrument(string texName, Vector2 position, 
            float minVal, float maxVal, float scaleVal, 
            string sourceName, string titleString, PlayerAirplane player)
        {
            _titleString = titleString;
            _texName = texName;
            minValue = minVal;
            maxValue = maxVal;
            _player = player;
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


        public void Update(GameTime gameTime)
        {
            if (_sourceName == "throttle")
            {
                currentValue = (float)_player.Thrust;
            }
            else if (_sourceName == "linearvelocity")
            {
                currentValue = (float)_player.LinearVelocity;
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Don't show UI if ShowUI is false
            // Don't show UI if player is crashing or is Dead.
            // Don't show UI if level is completed.
            if (GameSettings.Default.ShowUI && !LevelManager.CurrentLevel.Completed)
            {
                // Standardize input, calculate positions
                float max_sd = maxValue - minValue;
                float curr_sd = currentValue - minValue;
                float offset180multiplier = max_sd / 180;
                float curr_real = curr_sd / offset180multiplier;

                Texture2D tex = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;

                Vector2 scaledPos = pos - new Vector2(0, tex.Height * scale);
                Vector2 needle_origin = scaledPos + new Vector2(tex.Width * scale / 2, tex.Height * scale);

                ScreenManager.SpriteBatch.Begin();
                // Draw instrument
                ScreenManager.SpriteBatch.Draw(tex, scaledPos, null, Color.White, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                // Draw needle
                DrawLine(ScreenManager.SpriteBatch, needleTexture, 3, (texHeight - 15) * scale, curr_real - 90, Color.White, needle_origin);

                // Draw title-text
                Text.DrawText(
                            ScreenManager.Cartoon14, // Font
                            _titleString,   // Text
                            Color.White,    // Inner color
                            scaledPos + new Vector2(TextureManager.GetCenterX(0f,
                                                                    tex.Width * scale,
                                                                    ScreenManager.Cartoon14.MeasureString(_titleString).X), -20f),      // Position
                            1.3f);          // Outline thickness

                ScreenManager.SpriteBatch.End();
            }
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/UI/instrument"), TextureName);

            Texture2D texture = TextureManager.GetTexture(TextureName).BaseTexture as Texture2D;
            texHeight = texture.Height;
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(TextureName);
        }
    }
}
