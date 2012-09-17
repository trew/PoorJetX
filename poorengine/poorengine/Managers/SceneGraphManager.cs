using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject.SceneGraph;
using PoorEngine.SceneObject;

namespace PoorEngine.Managers
{
    public class SceneGraphManager : GameComponent
    {
        private static Node _root;
        /// <summary>
        /// The root of the scene graph
        /// </summary>
        public static Node Root
        {
            get { return _root; }
        }

        /// <summary>
        /// Create the scenegraph Managers.
        /// </summary>
        /// <param name="game"></param>
        public SceneGraphManager(Game game)
            : base(game)
        {
            _root = new Node();
        }

        /// <summary>
        /// Draw objects
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Draw(GameTime gameTime)
        {

            _root.Draw(gameTime);
        }

        public static void LoadContent()
        {

            _root.LoadContent();
        }

        public static void UnloadContent()
        {

            _root.UnloadContent();
        }

        public static void AddObject(PoorSceneObject newObject)
        {
            SceneObjectNode node = new SceneObjectNode(newObject);
            _root.AddNode(node);
        }

    }
}
