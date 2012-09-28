using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoorEngine.GameComponents;
using Microsoft.Xna.Framework;

namespace PoorEngine.Managers
{
    public class CameraManager : GameComponent
    {
        private static Camera _camera;

        public CameraManager(Game game)
            : base(game)
        {
            _camera = new Camera(new Vector2());
        }

        public static Camera Camera {
            get { return _camera; }
        }

        public static void Reset() {
            _camera.Pos = new Vector2();
        }
    }
}
