using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorEngine.Interfaces;

namespace PoorEngine.SceneObject.SceneGraph
{
    public class SceneObjectNode : Node
    {
        private PoorSceneObject _sceneObject;
        public PoorSceneObject SceneObject;

        public SceneObjectNode(PoorSceneObject newObject)
        {
            _sceneObject = newObject;
        }

        public override void Update()
        {
            if (SceneObject is IPoorUpdateable)
            {
                ((IPoorUpdateable)SceneObject).Update();
            }
        }
        public override void LoadContent()
        {
            if (SceneObject is IPoorLoadable)
            {
                ((IPoorLoadable)SceneObject).LoadContent();
            }
        }
        public override void UnloadContent()
        {
            if (SceneObject is IPoorLoadable)
            {
                ((IPoorLoadable)SceneObject).UnloadContent();
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if (SceneObject is IPoorDrawable)
            {
                ((IPoorDrawable)SceneObject).Draw(gameTime);
            }
        }
    }
}
