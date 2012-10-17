#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.SceneObject;
using PoorEngine.Managers;
using Microsoft.Xna.Framework;
#endregion

namespace PoorEngine.GameComponents
{
    [Serializable]
    public class LevelVisual
    {
        public bool Repeatable;
        public int RepeatMargin;
        public int X;
        public int Y;
        public bool AlignToBottom;
        public string FileName;
        public float Z;
        public float Scale;
        public float ConstantSpeed;
    }

    [Serializable]
    public class LevelText
    {
        public string text;
        public string fontName;
        public int R;
        public int G;
        public int B;
        public int X;
        public int Y;
        public int XAppear;
        public bool center;
        public float outline;
        public float secondsTTL;
    }

    [Serializable]
    public class LevelBackground
    {
        public string FileName;
        public float Z;
    }

    [Serializable]
    public class LevelEnemy
    {
        /// <summary>
        /// When player reached XAppear on the X-axis,
        /// spawn this enemy
        /// </summary>
        public int XAppear;

        /// <summary>
        /// Y-position of enemy
        /// </summary>
        public int Y;

        public int type;

        public int health;
    }

    [Serializable]
    public class LevelData
    {

        #region LevelData
        private List<LevelText> _texts;
        /// <summary>
        /// A list of text-objects
        /// </summary>
        public List<LevelText> Texts
        {
            get { return _texts; }
            set { _texts = value; }
        }

        private List<LevelVisual> _visuals;
        /// <summary>
        /// A list of visual objects
        /// </summary>
        public List<LevelVisual> Visuals
        {
            get { return _visuals; }
            set { _visuals = value; }
        }

        private List<LevelBackground> _backgrounds;
        /// <summary>
        /// A list of backgrounds where the String is the filename
        /// and float is the moveRatio
        /// </summary>
        public List<LevelBackground> Backgrounds
        {
            get { return _backgrounds; }
            set { _backgrounds = value; }
        }

        private List<LevelEnemy> _enemies;
        /// <summary>
        /// A list of enemies that spawn in this level
        /// </summary>
        public List<LevelEnemy> Enemies
        {
            get { return _enemies; }
            set { _enemies = value; }
        }
        #endregion

    }

    public class Level
    {
        private LevelData _data;

        public Level(LevelData data)
        {
            _data = data;
        }

        private Queue<EnemyAirplane> _enemies;
        private Queue<Text> _texts;

        #region Utility functions
        /// <summary>
        /// Load all backgrounds for this level and add them
        /// to the Scene using SceneGraphManager.
        /// </summary>
        public void LoadVisuals()
        {
            foreach (LevelBackground bg in _data.Backgrounds)
            {
                Background layer = new Background(bg.FileName, bg.Z);
                SceneGraphManager.AddObject(layer);
            }

            foreach (LevelVisual vs in _data.Visuals)
            {
                Visual visualObject = new Visual(vs.FileName, vs.Scale, vs.Z, vs.ConstantSpeed, vs.Repeatable, vs.RepeatMargin, new Vector2(vs.X, vs.Y));
                SceneGraphManager.AddObject(visualObject);
            }
        }

        /// <summary>
        /// Use all LevelEnemy objects and create
        /// EnemyAirplane . Queue them in a list.
        /// </summary>
        public void QueueEnemies()
        {
            // Sort enemies by XAppear, so they appear correctly
            // in the queue.
            _data.Enemies.Sort((LevelEnemy x1, LevelEnemy x2) =>
                x1.XAppear < x2.XAppear ? -1 : 1);

            _enemies = new Queue<EnemyAirplane>();
            foreach (LevelEnemy le in _data.Enemies)
            {
                EnemyAirplane e = new EnemyAirplane(le.health);
                e.Position = new Vector2(le.XAppear, le.Y);
                _enemies.Enqueue(e);
            }
        }

        /// <summary>
        /// Get the next enemy in queue
        /// </summary>
        /// <returns></returns>
        public EnemyAirplane GetNextEnemy()
        {
            if (_enemies == null || _enemies.Count <= 0) return null;
            return _enemies.First();
        }

        /// <summary>
        /// Spawns a new enemy
        /// </summary>
        /// <returns>An enemy, or null if no enemies is in queue</returns>
        public EnemyAirplane SpawnEnemy()
        {
            if (_enemies == null || _enemies.Count <= 0) return null;

            _enemies.First().Position += new Vector2(EngineManager.Device.Viewport.Width + 100, 0);
            SceneGraphManager.AddObject(_enemies.First());
            return _enemies.Dequeue();
        }

        /// <summary>
        /// 
        /// </summary>
        public void QueueTexts()
        {
            // Sort texts by XAppear, so they appear correctly
            // in the queue.
            _data.Texts.Sort((LevelText x1, LevelText x2) =>
                x1.XAppear < x2.XAppear ? -1 : 1);

            _texts = new Queue<Text>();
            foreach (LevelText lt in _data.Texts)
            {
                Text t = new Text(lt.text, lt.fontName, new Color(lt.R, lt.G, lt.B), new Vector2(lt.X, lt.Y), lt.XAppear, lt.center, lt.outline, lt.secondsTTL);
                _texts.Enqueue(t);

            }
        }

        /// <summary>
        /// Get the next text in queue
        /// </summary>
        /// <returns></returns>
        public Text GetNextText()
        {
            if (_texts == null || _texts.Count <= 0) return null;
            return _texts.First();
        }

        /// <summary>
        /// Spawns a new text
        /// </summary>
        /// <returns>An text, or null if no texts is in queue</returns>
        public Text SpawnText()
        {
            if (_texts == null || _texts.Count <= 0) return null;
            _texts.First().SpawnTime = DateTime.Now.TimeOfDay;
            SceneGraphManager.AddObject(_texts.First());
            return _texts.Dequeue();
        }
        #endregion

    }
}
