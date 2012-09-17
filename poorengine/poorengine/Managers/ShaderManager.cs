using System;

using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using PoorEngine.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using PoorEngine.Shaders;

namespace PoorEngine.Managers
{
    public class ShaderManager : GameComponent
    {
        private static Dictionary<string, IPoorShader> _shaders = new Dictionary<string,IPoorShader>();

        private static bool _initialized = false;
        /// <summary>
        /// Is the ShaderManager initialized, used for test cases and setup of Effects.
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// Create the shader Managers.
        /// </summary>
        /// <param name="game"></param>
        public ShaderManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Add a shader of type IPoorShader
        /// </summary>
        /// <param name="newShader"></param>
        /// <param name="shaderLabel"></param>
        public static void AddShader(IPoorShader newShader, string shaderLabel)
        {
            if (shaderLabel != null && _shaders.ContainsKey(shaderLabel))
            {
                _shaders.Add(shaderLabel, newShader);

                newShader.Initialize(EngineManager.Device);
            }
        }

        /// <summary>
        /// Get a shader of type IPoorShader
        /// </summary>
        /// <param name="shaderLabel"></param>
        /// <returns></returns>
        public static IPoorShader GetShader(string shaderLabel)
        {
            if (shaderLabel != null && _shaders.ContainsKey(shaderLabel))
            {
                return _shaders[shaderLabel];
            }
            return null;
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (IPoorShader shader in _shaders.Values)
            {
                if (!shader.ReadyToRender)
                {
                    shader.Initialize(EngineManager.Device);
                }
            }

            AddShader(new basicEffect(), "BasicEffect");

            _initialized = true;
        }

    }
}
