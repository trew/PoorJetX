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

        public static Queue<Node> removeQueue { get; set; }

        /// <summary>
        /// Create the scenegraph Managers.
        /// </summary>
        /// <param name="game"></param>
        public SceneGraphManager(Game game)
            : base(game)
        {
            _root = new Node();
            removeQueue = new Queue<Node>();
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
            _root.Update(gameTime);

            foreach (SceneObjectNode firstNode in _root.Nodes)
            {
                PoorSceneObject first = firstNode.SceneObject;
                if (!first.UsedInBoundingBoxCheck) continue;

                foreach (SceneObjectNode secondNode in _root.Nodes)
                {
                    PoorSceneObject second = secondNode.SceneObject;
                    if (first == second) continue;
                    if (!second.UsedInBoundingBoxCheck) continue;
                    if (first.BoundingBox.Intersects(second.BoundingBox))
                    {
                        if (first.GetType() == typeof(Airplane) && second.GetType() == typeof(Projectile) ||
                            first.GetType() == typeof(Projectile) && second.GetType() == typeof(Airplane))
                        {
                            Projectile p = (Projectile)(first.GetType() == typeof(Projectile) ? first : second);
                            if (p.CanCollideWithPlayer(gameTime))
                            {
                                first.Collide(second);
                                second.Collide(first);
                            }
                        }
                        else
                        {
                            first.Collide(second);
                            second.Collide(first);
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
            _root.AddNode(node);
            _root.Nodes.Sort(comp);
        }

        public static void RemoveObject(PoorSceneObject oldObject)
        {
            foreach (SceneObjectNode node in _root.Nodes)
            {
                if (node.SceneObject == oldObject)
                {
                    removeQueue.Enqueue(node);
                    return;
                }
            }
        }
    }
}