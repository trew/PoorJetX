﻿using System;
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

namespace PoorEngine.SceneObject
{
    public class Instrument: PoorSceneObject, IPoorDrawable, IPoorUpdateable, IPoorLoadable
    {
        float currentValue;
        float minValue;
        float maxValue;
        Vector2 pos;
        float scale;
        string textureName;
        Texture2D needleTexture;
        float texHeight;
        private string _sourceName;
        GamePlayScreen currentScreen;

        public Instrument(string texName, Vector2 position, 
            float minVal, float maxVal, float scaleVal, 
            string sourceName, GamePlayScreen gameScreen)
        {
            textureName = texName;
            minValue = minVal;
            maxValue = maxVal;
            currentScreen = gameScreen;
            pos = position;
            _sourceName = sourceName;
            scale = scaleVal;
            needleTexture = new Texture2D(EngineManager.Device, 1, 1, false, SurfaceFormat.Color);
            needleTexture.SetData(new[] { Color.White });
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        void DrawLine(SpriteBatch batch, Texture2D texture,
             float width, float length, float angle, Color color, Vector2 startPoint)
        {
            batch.Draw(texture,
                startPoint,
                null,
                color,
                (float)DegreeToRadian(angle - 90),
                Vector2.Zero,
                new Vector2(length, width),
                SpriteEffects.None, 0);
        }


        public void Update(GameTime gameTime)
        {
            if (_sourceName == "throttle")
            {
                currentValue = (float)currentScreen.Airplane.getThrottle();
            }
            else if (_sourceName == "linearvelocity")
            {
                currentValue = (float)currentScreen.Airplane.getLinearVelocity();
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Standardize input, calculate positions
            float max_sd = maxValue - minValue;
            float curr_sd = currentValue - minValue;
            float offset180multiplier = max_sd / 180;
            float curr_real = curr_sd / offset180multiplier;
         
            Texture2D tex = TextureManager.GetTexture(textureName).BaseTexture as Texture2D;

            Vector2 scaledPos = pos - new Vector2(0, tex.Height*scale);
            Vector2 needle_origin = scaledPos + new Vector2(tex.Width * scale / 2, tex.Height * scale);

            ScreenManager.SpriteBatch.Begin();
            // Draw instrument
            ScreenManager.SpriteBatch.Draw(tex, scaledPos, null, Color.White, 0f, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
            // Draw needle
            DrawLine(ScreenManager.SpriteBatch, needleTexture, 3, (texHeight - 15) * scale, curr_real - 90, Color.White, needle_origin);
            ScreenManager.SpriteBatch.End();
        }

        public void LoadContent()
        {
            TextureManager.AddTexture(new PoorTexture("Textures/instrument"), textureName);

            Texture2D texture = TextureManager.GetTexture(textureName).BaseTexture as Texture2D;
            texHeight = texture.Height;
        }

        public void UnloadContent()
        {
            TextureManager.RemoveTexture(textureName);
        }
    }
}