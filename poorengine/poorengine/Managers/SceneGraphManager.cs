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

        public static bool newObjectsAdded;
        public static Queue<Node> removeQueue { get; set; }
        private static Queue<Node> _new;

        /// <summary>
        /// Create the scenegraph Managers.
        /// </summary>
        /// <param name="game"></param>
        public SceneGraphManager(Game game)
            : base(game)
        {
            _root = new Node();
            _new = new Queue<Node>();
            removeQueue = new Queue<Node>();

            newObjectsAdded = false;
        }

        /// <summary>
        /// Draw objects
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Draw(GameTime gameTime)
        {
            _root.Draw(gameTime);
        }

        public static new void Update(GameTime gameTime)
        {
            // Add new nodes and sort the list
            if (newObjectsAdded)
            {
                while(_new.Count > 0)
                {
                    _root.AddNode(_new.Dequeue());

                }
                _root.Nodes.Sort(comp);

                newObjectsAdded = false;
            }
            _root.Update(gameTime);

            for (int first = 0; first < _root.Nodes.Count; first++)
            {
                PoorSceneObject firstObject = ((SceneObjectNode)_root.Nodes[first]).SceneObject;
                if (!firstObject.UsedInBoundingBoxCheck) continue;

                for(int second = 0; second < _root.Nodes.Count; second++)
                {
                    PoorSceneObject secondObject = ((SceneObjectNode)_root.Nodes[second]).SceneObject;
                    if (firstObject == secondObject) continue;
                    if (!secondObject.UsedInBoundingBoxCheck) continue;
                    if (firstObject.BoundingBox.Intersects(secondObject.BoundingBox))
                    {
                        if (firstObject.GetType() == typeof(PlayerAirplane) && secondObject.GetType() == typeof(Projectile) ||
                            firstObject.GetType() == typeof(Projectile) && secondObject.GetType() == typeof(PlayerAirplane))
                        {
                            Projectile p = (Projectile)(firstObject.GetType() == typeof(Projectile) ? firstObject : secondObject);
                            if (p.CanCollideWithPlayer(gameTime))
                            {
                                firstObject.Collide(secondObject);
                                secondObject.Collide(firstObject);
                            }
                        }
                        else
                        {
                            firstObject.Collide(secondObject);
                            secondObject.Collide(firstObject);
                        }
                    }
                }
            }

            // Remove all queued nodes
            while (removeQueue.Count > 0) {
                _root.Nodes.Remove(removeQueue.Dequeue());
            }

        }

        public static void LoadContent()
        {

            _root.LoadContent();
        }

        public static void UnloadContent()
        {

            _root.UnloadContent();
        }

        private static int comp(Node x1, Node x2)
        {
            SceneObjectNode node1 = (SceneObjectNode)x1;
            SceneObjectNode node2 = (SceneObjectNode)x2;

            if (node1.SceneObject.Z == node2.SceneObject.Z) return 0;
            return node1.SceneObject.Z > node2.SceneObject.Z ? -1 : 1;
        }

        public static void AddObject(PoorSceneObject newObject)
        {
            SceneObjectNode node = new SceneObjectNode(newObject);
            
            node.LoadContent(); // this or crash in node.Draw
            _new.Enqueue(node);

            newObjectsAdded = true;
        }

        public static void RemoveObject(PoorSceneObject oldObject)
        {
            foreach (SceneObjectNode node in _root.Nodes)
            {
                if (node.SceneObject == oldObject)
                {
                    if (removeQueue.Contains(node))
                        return;
                    removeQueue.Enqueue(node);
                    return;
                }
            }
        }
    }
}
