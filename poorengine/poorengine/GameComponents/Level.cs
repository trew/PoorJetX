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
    public class LevelBackground
    {
        public string fileName;
        public float moveRatio;
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
    }

    [Serializable]
    public class LevelData
    {

        #region LevelData
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

        #region Utility functions
        /// <summary>
        /// Load all backgrounds for this level and add them
        /// to the Scene using SceneGraphManager.
        /// </summary>
        public void LoadBackgrounds()
        {
            foreach (LevelBackground bg in _data.Backgrounds)
            {
                Background layer = new Background(bg.fileName, bg.moveRatio);
                SceneGraphManager.AddObject(layer);
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
                EnemyAirplane e = new EnemyAirplane();
                e.Position = new Vector2(le.XAppear, le.Y);
                _enemies.Enqueue(e);
            }
        }

        /// <summary>
        /// Spawns a new enemy if the camera has reached the x
        /// point for that enemy.
        /// </summary>
        /// <returns>An enemy, or null if no enemy is to be spawned</returns>
        public EnemyAirplane SpawnEnemy(float x)
        {
            if (_enemies == null || _enemies.Count <= 0) return null;

            if (_enemies.First().Position.X <= x)
            {
                SceneGraphManager.AddObject(_enemies.First());
                return _enemies.Dequeue();
            }
            return null;
        }
        #endregion

    }
}
