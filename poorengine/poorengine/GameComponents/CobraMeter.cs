using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.SceneObject;
using PoorEngine.Settings;
using PoorEngine.Managers;

namespace PoorEngine.GameComponents
{
    public class CobraMeter
    {
        PlayerAirplane _player;

        public Vector2 Position;

        Rectangle _meterRect;
        Rectangle _outlineRect;

        private int _height = 100;
        private int _width = 20;
        private int _outline = 2;

        public CobraMeter(PlayerAirplane player)
        {
            _player = player;
            Position = new Vector2(10, 300);
            _outlineRect = new Rectangle(9999, 9999, _width + _outline * 2, _height + _outline * 2);
            _meterRect = new Rectangle(9999, 9999, _width, _height);
        }

        public void Update(GameTime gameTime)
        {
        }

        private double CobraForcePercent { get { return _player.CobraForce / Airplane.MAXCOBRAFORCE; } }

        public void Draw(GameTime gameTime)
        {
            if (GameSettings.Default.ShowUI)
            {
                // Update Healthbar draw-settings.

                _meterRect.Height = (int)(_height * (_player.CobraForce / Airplane.MAXCOBRAFORCE));
                int red = (int)(255 - 255 * CobraForcePercent);
                int green = (int)(255 * CobraForcePercent);

                Color color = new Color(red * 3, green * 2, 0);

                Texture2D texBlack = TextureManager.GetColorTexture(Color.Black);
                Texture2D texHealth = TextureManager.GetColorTexture(color);

                ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(texBlack,
                                               Position - new Vector2(2, 2),
                                               _outlineRect,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
                ScreenManager.SpriteBatch.Draw(texHealth,
                                               Position + new Vector2(0, _height - _meterRect.Height),
                                               _meterRect,
                                               Color.White,
                                               0f,
                                               new Vector2(0, 0),
                                               1f,
                                               SpriteEffects.None,
                                               0f);
                ScreenManager.SpriteBatch.End();
            }
        }
    }
}
