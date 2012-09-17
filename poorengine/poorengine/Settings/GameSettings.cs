using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using PoorEngine.Helpers;

namespace PoorEngine.Settings
{
    [Serializable]
    public class GameSettings
    {
        #region Properties
        private static bool _needSave = false;

        private string _playerName = "PoorJet";
        /// <summary>
        /// Player name
        /// </summary>
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                if (_playerName != value)
                    _needSave = true;
                _playerName = value;
            }
        }

        public const int MinimumResolutionWidth = 640;

        private int _resolutionWidth = 0;
        /// <summary>
        /// Resolution Width
        /// </summary>
        public int ResolutionWidth
        {
            get { return _resolutionWidth; }
            set
            {
                if (_resolutionWidth != value)
                    _needSave = true;
                _resolutionWidth = value;
            }
        }

        public const int MinimumResolutionHeight = 480;

        private int _resolutionHeight = 0;
        /// <summary>
        /// Resolution Height
        /// </summary>
        public int ResolutionHeight
        {
            get { return _resolutionHeight; }
            set
            {
                if (_resolutionHeight != value)
                    _needSave = true;
                _resolutionHeight = value;
            }
        }

        private bool _fullScreen = false;
        /// <summary>
        /// Fullscreen.
        /// </summary>
        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                if (_fullScreen != value)
                    _needSave = true;
                _fullScreen = value;
            }
        }

        private float _soundVolume = 0.8f;
        /// <summary>
        /// General sound volume
        /// </summary>
        public float SoundVolume
        {
            get { return _soundVolume; }
            set
            {
                if (_soundVolume != value)
                    _needSave = true;
                _soundVolume = value;
            }
        }

        private float _musicVolume = 0.8f;
        /// <summary>
        /// Music volume
        /// </summary>
        public float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (_musicVolume != value)
                    _needSave = true;
                _musicVolume = value;
            }
        }
        #endregion

        #region Default
        /// <summary>
        /// Filename used to store the game settings
        /// </summary>
        const string SettingsFilename = "GameSettings.xml";

        private static GameSettings _defaultInstance = null;
        /// <summary>
        /// Default instance of the game settings
        /// </summary>
        public static GameSettings Default
        {
            get { return _defaultInstance; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// No public constructor! Create the game settings
        /// </summary>
        private GameSettings()
        {
        }

        public static void Initialize()
        {
            _defaultInstance = new GameSettings();
            Load();
        }
        #endregion

        #region Load
        /// <summary>
        /// Load
        /// </summary>
        public static void Load()
        {
            _needSave = false;
            FileStream file = FileHelper.LoadGameContentFile(
                SettingsFilename);

            if (file == null)
            {
                _needSave = true;
                return;
            }

            // If the file is empty, just create a new file with the default
            // settings.
            if (file.Length == 0)
            {
                throw new Exception("Not yet finished.");
            }
            else
            {
                // Load everything into this class with the help of
                // the xmlserializer
                GameSettings loadedGameSettings =
                    (GameSettings)new XmlSerializer(typeof(GameSettings)).Deserialize(file);
                file.Close();

                if (loadedGameSettings != null)
                    _defaultInstance = loadedGameSettings;
            }
        }
        #endregion

        #region Save
        /// <summary>
        /// Save
        /// </summary>
        public static void Save()
        {
            if (!_needSave)
                return;

            _needSave = false;
            FileStream file = FileHelper.SaveGameContentFile(
                SettingsFilename);

            // Save everything in this class with help from the XmlSerializer
            new XmlSerializer(typeof(GameSettings)).Serialize(file, _defaultInstance);

            file.Close();
        }
        #endregion

        #region Set Minimum Graphics
        /// <summary>
        /// Set minimum graphic capabilities
        /// </summary>
        public static void SetMinimumGraphics()
        {
            GameSettings.Default.ResolutionWidth = GameSettings.MinimumResolutionWidth;
            GameSettings.Default.ResolutionHeight = GameSettings.MinimumResolutionHeight;
            GameSettings.Save();
        }
        #endregion
    }
}
