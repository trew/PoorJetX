#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.SceneObject;
using PoorEngine.Managers;
using Microsoft.Xna.Framework;
using PoorEngine.Helpers;
using PoorEngine.Interfaces;
#endregion

namespace PoorEngine.GameComponents
{
    [Serializable]
    public class TimeOfDay
    {
        public int Hour;
    }

    [Serializable]
    public class LevelBriefing
    {
        public string Title;
        public string Story;
        public List<LevelObjective> Objectives;
    }

    [Serializable]
    public class LevelObjective
    {
        public string Description;
    }

    [Serializable]
    public class LevelVisual
    {
        public bool Repeatable;
        public int RepeatMargin;
        public int X;
        public int Y;
        public float Scale;
        public string FileName;
        public float Z;
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
        public float YZ;
        public float Height;
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

        public string type;

        public int health;

        public bool RequiredForVictory;
    }

    [Serializable]
    public class LevelObject
    {
        public string type;
        public int XAppear;
    }

    [Serializable]
    public class LevelData
    {

        #region LevelData
        private LevelBriefing _briefing;
        public LevelBriefing Briefing
        {
            get { return _briefing; }
            set { _briefing = value; }
        }

        private TimeOfDay _timeOfDay;
        public TimeOfDay TimeOfDay
        {
            get { return _timeOfDay; }
            set { _timeOfDay = value; }
        }

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

        private List<LevelObject> _objects;
        /// <summary>
        /// A list of objects that spawn in this level
        /// </summary>
        public List<LevelObject> Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }
        #endregion

    }

    public class Level
    {
        private LevelData _data;
        private bool _loaded = false;
        public bool Loaded { get { return _loaded; } }

        private bool _completed = false;
        public bool Completed { get { return _completed; } set { _completed = value; } }

        private int _levelNumber = 0;
        public int LevelNumber { get { return _levelNumber; } }

        public Level(LevelData data, int levelNumber)
        {
            _data = data;
            _levelNumber = levelNumber;
        }

        private LevelBriefing _briefing;
        public LevelBriefing Briefing { get { return _briefing; } }
        private Queue<PoorSceneObject> _enemies;
        private Queue<Text> _texts;
        private Queue<LevelObject> _objects;
        private List<IPoorEnemy> _aliveEnemies; // used for victory conditions

        #region Utility functions
        public void Load()
        {
            _aliveEnemies = new List<IPoorEnemy>();
            _briefing = _data.Briefing;
            LoadVisuals();
            QueueEnemies();
            QueueTexts();
            QueueObjects();
            // TODO QueueSounds();
            SceneGraphManager.SetTimeOfDay24h = (float)_data.TimeOfDay.Hour;
            _loaded = true;
            _completed = false;
        }

        /// <summary>
        /// Load all backgrounds for this level and add them
        /// to the Scene using SceneGraphManager.
        /// </summary>
        public void LoadVisuals()
        {
            foreach (LevelBackground bg in _data.Backgrounds)
            {
                Background layer = new Background(bg.FileName, bg.Z, bg.YZ, bg.Height);
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

            _enemies = new Queue<PoorSceneObject>();
            foreach (LevelEnemy le in _data.Enemies)
            {
                PoorSceneObject e = null;
                if (le.type == "Airplane")
                {
                    e = new EnemyAirplane(le.health, le.RequiredForVictory);
                    e.Position = new Vector2(le.XAppear, GameHelper.GroundLevel - 70 - le.Y);
                }
                else if (le.type == "AntiAir")
                {
                    e = new AntiAirVehicle(le.health, le.RequiredForVictory);
                    e.Position = new Vector2(le.XAppear, GameHelper.GroundLevel - 49);
                }
                else if (le.type == "Transport")
                {
                    e = new GroundTransport(le.health, le.RequiredForVictory);
                    e.Position = new Vector2(le.XAppear, GameHelper.GroundLevel - 39);
                }
                else if (le.type == "BossAntiAir")
                {
                    e = new BossAntiAir(le.health, le.RequiredForVictory);
                    e.Position = new Vector2(le.XAppear, GameHelper.GroundLevel - 170);
                }
                else if (le.type == "BurgerBoss")
                {
                    e = new GroundTransport(le.health, le.RequiredForVictory, true);
                    e.Position = new Vector2(le.XAppear, GameHelper.GroundLevel - 130);
                }

                if (e != null)
                    _enemies.Enqueue(e);
            }
        }

        /// <summary>
        /// Get the next enemy in queue
        /// </summary>
        /// <returns></returns>
        public PoorSceneObject GetNextEnemy()
        {
            if (_enemies == null || _enemies.Count <= 0) return null;
            return _enemies.First();
        }

        /// <summary>
        /// Spawns a new enemy
        /// </summary>
        /// <returns>An enemy, or null if no enemies is in queue</returns>
        public PoorSceneObject SpawnEnemy()
        {
            if (_enemies == null || _enemies.Count <= 0) return null;

            _enemies.First().Position += new Vector2(GameHelper.ScreenWidth + 100, 0);
            SceneGraphManager.AddObject(_enemies.First());
            _aliveEnemies.Add((IPoorEnemy)_enemies.First());
            return _enemies.Dequeue();
        }

        public bool RemoveEnemy(IPoorSceneObject enemy)
        {
            return _aliveEnemies.Remove((IPoorEnemy)enemy);
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



        
        /// <summary>
        /// 
        /// </summary>
        public void QueueObjects()
        {
            // Sort objects by XAppear, so they appear correctly
            // in the queue.
            _data.Objects.Sort((LevelObject x1, LevelObject x2) =>
                x1.XAppear < x2.XAppear ? -1 : 1);

            _objects = new Queue<LevelObject>();
            foreach (LevelObject lt in _data.Objects)
            {
                _objects.Enqueue(lt);
            }
        }

        /// <summary>
        /// Get the next object in queue
        /// </summary>
        /// <returns></returns>
        public LevelObject GetNextObject()
        {
            if (_objects == null || _objects.Count <= 0) return null;
            return _objects.First();
        }

        /// <summary>
        /// Spawns a new object
        /// </summary>
        /// <returns>An object, or null if no object is in queue</returns>
        public LevelObject SpawnObject()
        {
            if (_objects == null || _objects.Count <= 0) return null;
            
            if (_objects.First().type.Equals("ammobase"))
            {
                AmmoBase ab = new AmmoBase();
                ab.Position = new Vector2(
                                            CameraManager.Camera.Pos.X + GameHelper.ScreenWidth + 250f,
                                            GameHelper.GroundLevel + 10);
                SceneGraphManager.AddObject(ab);
            }
            else
                return null;

            return _objects.Dequeue();
        }


        /// <summary>
        /// Pretty much the victory conditions for our game.
        /// </summary>
        /// <returns></returns>
        public bool CheckCompleted()
        {
            if (_enemies.Count > 0) return false;
            foreach (IPoorEnemy enemy in _aliveEnemies)
            {
                if (!enemy.IsDead && enemy.RequiredForVictory)
                    return false;
            }
            _completed = true;
            return true;
        }

        #endregion

    }
}
