using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject;

namespace PoorEngine.Managers
{
    public class AIManager: GameComponent
    {
        private static AIManager _instance = null;
        public static AIManager GetAIManager(Game game)
        {
            if (_instance == null)
            {
                _instance = new AIManager(game);
            }
            return _instance;
        }
        protected AIManager(Game game) :
            base(game)
        {
            _objects = new List<PoorSceneObject>();
        }

        private static List<PoorSceneObject> _objects;

        public static void Reset()
        {
            _objects.Clear();
        }
    
        public static void RegisterObject(PoorSceneObject obj)
        {
            _objects.Add(obj);
        }

        public static void UnregisterObject(PoorSceneObject obj)
        {
            _objects.Remove(obj);
        }

        public static void Update(GameTime gameTime, PoorSceneObject obj)
        {
        }
    }
}
