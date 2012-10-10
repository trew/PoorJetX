using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
namespace PoorEngine.Managers
{
    public class SoundFxLibrary : GameComponent
    {
        private static Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>();

        private static int _soundEffectsLoaded = 0;
        /// <summary>
        /// The number of textures that are currently loaded.
        /// Use this for user loading bar feedback
        /// </summary>
        public static int SoundEffectsLoaded
        {
            get { return _soundEffectsLoaded; }
        }

        public SoundFxLibrary(Game game)
            : base(game)
        {
        }

        public static void AddToLibrary(string filePath, string FxName)
        {
            if (FxName != null && !_soundEffects.ContainsKey(FxName))
            {
                _soundEffects.Add(FxName, EngineManager.Game.Content.Load<SoundEffect>(filePath));
                _soundEffectsLoaded++;
            }
        }


        public static void RemoveFx(string fxName)
        {
            if (fxName != null && _soundEffects.ContainsKey(fxName))
            {
                _soundEffects.Remove(fxName);
                _soundEffectsLoaded--;
            }
        }

        public static SoundEffect GetFx(string fxName)
        {
            if (fxName != null && _soundEffects.ContainsKey(fxName))
            {
                return _soundEffects[fxName];
            }
            return null;
        }

        public static SoundEffectInstance GenerateInstance(string fxName)
        {
            if (fxName != null && _soundEffects.ContainsKey(fxName))
            {
                return _soundEffects[fxName].CreateInstance();
            }
            return null;
        }

    }
}
