using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.SceneObject;
using PoorEngine.Managers;

namespace PoorEngine.GameComponents
{
    [Serializable]
    public class LevelBackground
    {
        public string fileName;
        public float moveRatio;
    }

    [Serializable]
    public class Level
    {
        // bakgrund 1,2 och 3

        // lista med objekt och när de spawnar?
        // object: x,y värde. typ av objekt. 

        // boss?

        private List<LevelBackground> _backgrounds;
        /// <summary>
        /// A list of backgrounds where the String is the filename
        /// and float is the moveRatio
        /// </summary>
        public List<LevelBackground> Backgrounds
        {
            get
            {
                return _backgrounds;
            }
            set
            {
                _backgrounds = value;
            }
        }

        public void LoadBackgrounds()
        {
            foreach (LevelBackground bg in _backgrounds) {
                Background layer = new Background(bg.fileName, bg.moveRatio);
                SceneGraphManager.AddObject(layer);
            }
        }
    }
}
