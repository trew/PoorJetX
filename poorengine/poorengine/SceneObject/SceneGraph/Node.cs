using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PoorEngine.SceneObject.SceneGraph
{
    public class Node
    {
        protected NodeList _nodes;
        public NodeList Nodes
        {
            get { return _nodes; }
        }

        public Node()
        {
            _nodes = new NodeList();
        }

        public void AddNode(Node newNode)
        {
            _nodes.Add(newNode);
        }

        public virtual void Update()
        {
            _nodes.ForEach(
                delegate(Node node)
                {
                    node.Update();
                }
            );
        }

        public virtual void UnloadContent()
        {
            _nodes.ForEach(
                delegate(Node node)
                {
                    node.UnloadContent();
                }
            );
        }

        public virtual void LoadContent()
        {
            _nodes.ForEach(
                delegate(Node node)
                {
                    node.LoadContent();
                }
            );
        }

        public virtual void Draw(GameTime gameTime)
        {
            _nodes.ForEach(
                delegate(Node node)
                {
                    node.Draw(gameTime);
                }
            );
        }
    }
}
