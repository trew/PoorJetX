using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PoorEngine.Settings;
using PoorEngine.Managers;
using PoorEngine.GameComponents;
using PoorEngine.Particles;
using PoorEngine.SceneObject;
using PoorEngine.GameScreens;

namespace PoorEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class PoorEngine : Game
    {
        protected static GraphicsDeviceManager _graphicsDeviceManager = null;
        /// <summary>
        /// Graphics Device
        /// </summary>
        public static GraphicsDevice Device
        {
            get { return _graphicsDeviceManager.GraphicsDevice; }
        }

        private bool _isAppActive = false;
        /// <summary>
        /// Is the application active?
        /// </summary>
        public bool IsAppActive
        {
            get { return _isAppActive; }
            set { _isAppActive = value; }
        }

        protected SpriteBatch _spriteBatch = null;
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
            set { _spriteBatch = value; }
        }

        private static Input _input = null;
        /// <summary>
        /// The input helper for menus, keyboard and mouse
        /// </summary>
        public static Input Input
        {
            get { return _input; }
        }

        private static ScreenManager _screenManagers = null;
        private static TextureManager _textureManagers = null;
        private static SceneGraphManager _sceneGraphManagers = null;

        private static CameraManager _cameraManager = null;
        private static LevelManager _levelManagers = null;
        private static bool _checkedGraphicsOptions = false;
        private static bool _applyDeviceChanges = false;

        public static DebugPanel Debug = null;

        private static ParticleManager _particleManagers = null;
        public static int Score = 0;

        /// <summary>
        /// Create Poor Engine
        /// </summary>
        /// <param name="windowsTitle"></param>
        protected PoorEngine(string windowsTitle)
        {
            this.Window.Title = "PoorJet X Beta";
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            GameSettings.Initialize();

            ApplyResolutionChange();

            //_graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

            // Demand to update as fast as possible, do not use fixed time steps.
            // The whole game is designed this way, if you remove this line
            // the game will not behave normal any longer!
            //this.IsFixedTimeStep = false;

            // Init texture managers
            _textureManagers = new TextureManager(this);
            Components.Add(_textureManagers);

            // Init the input
            _input = new Input(this);
            Components.Add(_input);

            // Init the screen managers
            _screenManagers = new ScreenManager(this);
            Components.Add(_screenManagers);

            // Init scene graph managers
            _sceneGraphManagers = new SceneGraphManager(this);
            Components.Add(_sceneGraphManagers);

            // Init camera manager
            _cameraManager = new CameraManager(this);
            Components.Add(_cameraManager);

            // Init the level managers
            _levelManagers = new LevelManager(this);
            Components.Add(_levelManagers);

            Debug = new DebugPanel(this);
            Components.Add(Debug);

            _particleManagers = new ParticleManager(this);
            Components.Add(_particleManagers);

            Content.RootDirectory = "Content";

            Song bgMusic = Content.Load<Song>("DST-FlyingCobra");
            MediaPlayer.Volume = SoundFxManager.GetVolume("Music", 1.0f);
            MediaPlayer.Play(bgMusic);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Create Poor Engine
        /// </summary>
        protected PoorEngine() : this("Game")
        {
        }

        public static PlayerAirplane Player
        {
            // OMFG SO UGLY!
            // Indeed :S
            get
            {
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    if (SceneGraphManager.TypeMatch(screen.GetType(), typeof(GamePlayScreen)))
                    {
                        GamePlayScreen gpScreen = (GamePlayScreen)screen;
                        return gpScreen.Airplane;
                    }
                }
                return null;
            }
        }

        public static void ApplyResolutionChange()
        {
            int resolutionWidth = GameSettings.Default.ResolutionWidth;
            int resolutionHeight = GameSettings.Default.ResolutionHeight;

            if (resolutionWidth <= 0 || resolutionHeight <= 0)
            {
                resolutionWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolutionHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            _graphicsDeviceManager.PreferredBackBufferWidth = resolutionWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = resolutionHeight;
            _graphicsDeviceManager.IsFullScreen = GameSettings.Default.FullScreen;

            _applyDeviceChanges = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            GameSettings.Save();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Input.CurrentKeyboardState.IsKeyDown(Keys.F7))
            {
                SoundFxManager.SetVolume("Global", GameSettings.Default.GlobalSoundVolume - 0.01f);
            }
            else if (Input.CurrentKeyboardState.IsKeyDown(Keys.F8))
            {
                SoundFxManager.SetVolume("Global", GameSettings.Default.GlobalSoundVolume + 0.01f);
            }
            if (Input.CurrentKeyboardState.IsKeyDown(Keys.F9))
            {
                SoundFxManager.SetVolume("Sound", GameSettings.Default.SoundVolume - 0.01f);
            }
            else if (Input.CurrentKeyboardState.IsKeyDown(Keys.F10))
            {
                SoundFxManager.SetVolume("Sound", GameSettings.Default.SoundVolume + 0.01f);
            }
            if (Input.CurrentKeyboardState.IsKeyDown(Keys.F11))
            {
                SoundFxManager.SetVolume("Music", GameSettings.Default.MusicVolume - 0.01f);
            } else if (Input.CurrentKeyboardState.IsKeyDown(Keys.F12)) {
                SoundFxManager.SetVolume("Music", GameSettings.Default.MusicVolume + 0.01f);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_applyDeviceChanges)
            {
                _graphicsDeviceManager.ApplyChanges();
                _applyDeviceChanges = false;
            }
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            IsAppActive = true;
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            IsAppActive = false;
        }
    }
}
