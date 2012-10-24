using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.SceneObject.SceneGraph;
using PoorEngine.SceneObject;
using PoorEngine.Interfaces;
using PoorEngine.Helpers;

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

        private static float _timeOfDay;
        private static float _TODspeed;
        public static float SetTimeOfDay24h {
            set {
                _timeOfDay = ((value % 12) / 12f) * (float)MathHelper.Pi;
                if (value >= 12f)
                    _TODspeed = -Math.Abs(_TODspeed);
                else
                    _TODspeed = Math.Abs(_TODspeed);
            }
        }
        public static float TimeOfDay { get { return _timeOfDay; } set { _timeOfDay = value; } }
        public static float TImeOfDaySpeed { get { return _TODspeed; } set { _TODspeed = value; } }
        public static Color TODcolor { get { return new Color((int)(50 + ((_timeOfDay / MathHelper.Pi) * 205f)), (int)(120 + ((_timeOfDay / MathHelper.Pi) * 135f)), (int)(120 + ((_timeOfDay / MathHelper.Pi) * 135f))); } }

        public static bool newObjectsAdded;
        public static Queue<Node> removeQueue { get; set; }
        private static Queue<Node> _new;
        private static List<Pair<PoorSceneObject, PoorSceneObject>> _collidedObjects;
        private static bool _useCollisionDetection;
        public static void ToggleCollisionDetection() { _useCollisionDetection = !_useCollisionDetection; }

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

            _timeOfDay = MathHelper.Pi;
            _TODspeed = 0.0015f;

            _useCollisionDetection = true;

            newObjectsAdded = false;
            _collidedObjects = new List<Pair<PoorSceneObject, PoorSceneObject>>();
        }

        /// <summary>
        /// Draw objects
        /// </summary>
        /// <param name="gameTime"></param>
        public static void Draw(GameTime gameTime)
        {
            _root.Draw(gameTime);
        }

        public static bool TypeMatch(Type a, Type b)
        {
            return a.IsAssignableFrom(b) || b.IsAssignableFrom(a);
        }

        public static new void Update(GameTime gameTime)
        {
            updateTimeOfDay();

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

            if (_useCollisionDetection)
            {
                Pair<PoorSceneObject, PoorSceneObject> pair;
                for (int first = 0; first < _root.Nodes.Count; first++)
                {
                    PoorSceneObject firstObject = ((SceneObjectNode)_root.Nodes[first]).SceneObject;
                    if (!firstObject.UsedInBoundingBoxCheck) continue;

                    for (int second = 0; second < _root.Nodes.Count; second++)
                    {
                        PoorSceneObject secondObject = ((SceneObjectNode)_root.Nodes[second]).SceneObject;

                        // If we're comparing the same object, continue
                        if (firstObject.Equals(secondObject)) continue;

                        // If both of the objects is Projectiles, skip detection
                        if (TypeMatch(firstObject.GetType(), typeof(Projectile)) && TypeMatch(secondObject.GetType(), typeof(Projectile))) continue;

                        // If the second object isn't used for bounding box check, continue
                        if (!secondObject.UsedInBoundingBoxCheck) continue;

                        // Do the objects collide?
                        if (firstObject.BoundingBox.Intersects(secondObject.BoundingBox))
                        {
                            // Check whether this pair has already collided.
                            pair = new Pair<PoorSceneObject, PoorSceneObject>(firstObject, secondObject);

                            if (_collidedObjects.Contains(pair))
                            {
                                pair = null;
                                continue;
                            }
                            _collidedObjects.Add(pair);

                            if (TypeMatch(firstObject.GetType(), typeof(Projectile)) && TypeMatch(secondObject.GetType(), typeof(IPoorWeaponHolder)) ||
                                TypeMatch(firstObject.GetType(), typeof(IPoorWeaponHolder)) && TypeMatch(secondObject.GetType(), typeof(Projectile)))
                            {
                                // Separate projectile and the other object
                                Projectile p = (Projectile)(TypeMatch(firstObject.GetType(), typeof(Projectile)) ? firstObject : secondObject);
                                IPoorSceneObject obj = p == firstObject ? secondObject : firstObject;
                                if (p.CanCollideWithObject(gameTime, obj))
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


                _collidedObjects.Clear();
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
                    if (TypeMatch(node.SceneObject.GetType(), typeof(IPoorEnemy)))
                    {
                        LevelManager.CurrentLevel.RemoveEnemy(node.SceneObject);
                    }
                    return;
                }
            }
        }


        private static void updateTimeOfDay()
        {
            if (_timeOfDay >= MathHelper.Pi)
                _TODspeed = -Math.Abs(_TODspeed);
            
            
            else if (_timeOfDay <= 0)
                _TODspeed = Math.Abs(_TODspeed);
            
            _timeOfDay += _TODspeed;
        }
    }
}
