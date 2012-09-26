using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Textures;
using PoorEngine.Interfaces;
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
