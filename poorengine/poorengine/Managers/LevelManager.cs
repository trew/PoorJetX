﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.GameComponents;
using Microsoft.Xna.Framework;
using PoorEngine.Helpers;
using System.IO;
using System.Xml.Serialization;
using PoorEngine.SceneObject;

namespace PoorEngine.Managers
{
    public class LevelManager : GameComponent
    {

        private static String _levelPathPrefix = "level/";

        public LevelManager(Game game)
            : base(game)
        {
            Load(1);
        }

        private static Level _currentLevel;
        public static Level CurrentLevel
        {
            get { return _currentLevel; }
        }

        #region LoadLevel
        /// <summary>
        /// Load
        /// </summary>
        public static void Load(int level)
        {
            String filename = _levelPathPrefix + level.ToString() + ".xml";
            FileStream file = FileHelper.LoadGameContentFile(
                filename);

            // If the file is empty, just create a new file with the default
            // settings.
            if (file == null || file.Length == 0)
            {
                throw new Exception("Not yet finished.");
            }
            else
            {
                // Load everything into this class with the help of
                // the xmlserializer
                LevelData loadedLevelData = null;
                loadedLevelData =
                    (LevelData)new XmlSerializer(typeof(LevelData)).Deserialize(file);

                if (loadedLevelData != null)
                {
                    Level loadedLevel = new Level(loadedLevelData);
                    _currentLevel = loadedLevel;
                }
                else
                {
                    // Can this even happen if exceptions
                    // are handled?
                }
            }
        }
        #endregion
        #region SaveLevel
        /// <summary>
        /// Save
        /// </summary>
        public static void Save(Level level, int levelN)
        {
            String filename = _levelPathPrefix + levelN.ToString() + ".xml";
            FileStream file = FileHelper.SaveGameContentFile(
                filename);

            // Save everything in this class with help from the XmlSerializer
            new XmlSerializer(typeof(Level)).Serialize(file, level);

            file.Close();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}