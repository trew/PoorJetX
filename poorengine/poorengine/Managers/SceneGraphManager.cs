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

        private static Node _new;
        /// <summary>
        /// New nodes to be added in next update
        /// </summary>
        public static Node New
        {
            get { return _new; }
        }

        public static bool newObjectsAdded;
        public static Queue<Node> removeQueue { get; set; }

        /// <summary>
        /// Create the scenegraph Managers.
        /// </summary>
        /// <param name="game"></param>
        public SceneGraphManager(Game game)
            : base(game)
        {
            _root = new Node();
            _new = new Node();
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
                foreach (SceneObjectNode node in _new.Nodes)
                {
                    _root.AddNode(node);

                }
                _new.Nodes.Clear();
                _root.Nodes.Sort(comp);

                newObjectsAdded = false;
            }
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
                        if (first.GetType() == typeof(PlayerAirplane) && second.GetType() == typeof(Projectile) ||
                            first.GetType() == typeof(Projectile) && second.GetType() == typeof(PlayerAirplane))
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
            
            node.LoadContent(); // this or crash in node.Draw
            _new.AddNode(node);

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
