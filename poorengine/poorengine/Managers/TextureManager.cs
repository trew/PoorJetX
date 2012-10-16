using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Textures;
using PoorEngine.Interfaces;
using PoorEngine.GameComponents;

using System.Collections;

namespace PoorEngine.Managers
{
    public class TextureManager : GameComponent
    {
        private static Dictionary<string, IPoorTexture> _textures = new Dictionary<string, IPoorTexture>();

        private static bool _initialized = false;
        /// <summary>
        /// Is the texture manager initialized? Used for test cases and setup of effects.
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static int _texturesLoaded = 0;
        /// <summary>
        /// The number of textures that are currently loaded.
        /// Use this for user loading bar feedback
        /// </summary>
        public static int TexturesLoaded
        {
            get { return _texturesLoaded; }
        }

        /// <summary>
        /// Create the TextureManager.
        /// </summary>
        /// <param name="game"></param>
        public TextureManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Add a texture of type PoorTexture
        /// </summary>
        /// <param name="newTexture"></param>
        /// <param name="textureName"></param>
        public static void AddTexture(PoorTexture newTexture, string textureName)
        {
            if (textureName != null && !_textures.ContainsKey(textureName))
            {
                _textures.Add(textureName, newTexture);
                _texturesLoaded++;
                if (_initialized)
                {
                    newTexture.LoadContent();
                }
            }
        }

        /// <summary>
        /// Removes a texture
        /// </summary>
        /// <param name="textureName"></param>
        public static void RemoveTexture(string textureName)
        {
            if (textureName != null && _textures.ContainsKey(textureName))
            {
                if (_initialized)
                {
                    _textures[textureName].UnloadContent();
                    _textures.Remove(textureName);
                    _texturesLoaded--;
                }
            }
        }

        public static Rectangle GetCenterRectangle(int width, int height, Rectangle outerBorder)
        {
            Rectangle border = new Rectangle(outerBorder.X + (outerBorder.Width / 2 - width / 2),
                                             outerBorder.Y + (outerBorder.Height / 2 - height / 2),
                                             width, height);
            return border;
        }

        public static float GetCenterX(float x, float width, float textureWidth)
        {
            return x + width / 2 - textureWidth / 2;
        }

        public static Texture2D GetColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(EngineManager.Device, 1, 1);
            tex.SetData(new Color[] { color });
            return tex;
        }

        public static void DrawRectangle(SpriteBatch sb, Rectangle rec, int thickness, Color color, Camera cam)
        {
            Vector2 pos = cam.Normalize(new Vector2(rec.X, rec.Y));
            Rectangle newRec = new Rectangle((int)pos.X, (int)pos.Y, rec.Width, rec.Height);
            DrawRectangle(sb, newRec, thickness, color);
        }
        public static void DrawRectangle(SpriteBatch sb, Rectangle rec, int thickness, Color color)
        {
            Texture2D tex = GetColorTexture(color);
            //Top line
            sb.Draw(tex, new Rectangle(rec.X, rec.Y, rec.Width, thickness), color);
            //Left line
            sb.Draw(tex, new Rectangle(rec.X, rec.Y, thickness, rec.Height), color);
            
            //Bottom line
            sb.Draw(tex, new Rectangle(rec.X, rec.Y + rec.Height, rec.Width + thickness, thickness), color);
            //Right line
            sb.Draw(tex, new Rectangle(rec.X + rec.Width, rec.Y, thickness, rec.Height + thickness), color);
        }

        /// <summary>
        /// Gets a texture
        /// </summary>
        /// <param name="textureName"></param>
        /// <returns></returns>
        public static IPoorTexture GetTexture(string textureName)
        {
            if (textureName != null && _textures.ContainsKey(textureName))
            {
                return _textures[textureName];
            }
            return null;
        }

        /// <summary>
        /// Create the textures
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (IPoorTexture texture in _textures.Values)
            {
                if (!texture.ReadyToRender)
                {
                    texture.LoadContent();
                }
            }

            _initialized = true;
        }
    }
}
