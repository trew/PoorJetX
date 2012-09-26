using System;
using System.Collections.Generic;
using System.Text;
using PoorEngine.Interfaces;
using PoorEngine.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace PoorEngine.Textures
{
    public class PoorTexture : IPoorTexture
    {
        private string _fileName;
        /// <summary>
        /// The file name of the asset
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private Texture _baseTexture;
        /// <summary>
        /// Gets the underlying texture
        /// </summary>
        public Texture BaseTexture
        {
            get { return _baseTexture; }
        }

        private bool _readyToRender;
        /// <summary>
        /// Is the texture ready to be rendered=
        /// </summary>
        public bool ReadyToRender
        {
            get { return _readyToRender; }
        }

        /// <summary>
        /// Contruct a new PoorTexture
        /// </summary>
        public PoorTexture()
        {
        }

        public PoorTexture(string fileName)
        {
            _fileName = fileName;
        }

        public void LoadContent()
        {
            if (!String.IsNullOrEmpty(_fileName))
            {
                _baseTexture = EngineManager.Game.Content.Load<Texture>(_fileName);
                _readyToRender = true;
            }
        }

        public void UnloadContent()
        {
        }
    }
}
