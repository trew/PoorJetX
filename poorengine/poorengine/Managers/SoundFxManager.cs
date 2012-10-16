using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace PoorEngine.Managers
{
    public class SoundFxManager : GameComponent
    {
        private static Dictionary<int, SoundEffectInstance> _soundEffects = new Dictionary<int, SoundEffectInstance>();


        private static int _soundInstancesLoaded = 0;
        private static int _idCounter = 0;
        /// <summary>
        /// The number of textures that are currently loaded.
        /// Use this for user loading bar feedback
        /// </summary>
        public static int SoundEffectsLoaded
        {
            get { return _soundInstancesLoaded; }
        }

        public SoundFxManager(Game game)
            : base(game)
        {
        }

        public static int AddInstance(SoundEffectInstance instance)
        {
            _idCounter++;
            _soundEffects.Add(_idCounter, instance);
            _soundInstancesLoaded++;
            return _idCounter;
        }

        public static void RemoveFx(int id)
        {
            if (_soundEffects.ContainsKey(id))
            {
                _soundEffects.Remove(id);
                _soundInstancesLoaded--;
            }
        }

        public static SoundEffectInstance GetByID(int id)
        {
            if (_soundEffects.ContainsKey(id))
            {
                return _soundEffects[id];
            }
            return null;
        }

        public static void Pause()
        {
            foreach (SoundEffectInstance i in _soundEffects.Values)
            {
                i.Pause();
            }
        }

        public static void Resume()
        {
            foreach (SoundEffectInstance i in _soundEffects.Values)
            {
                if (i.State == SoundState.Paused)
                    i.Resume();
            }
        }

        public static void Clear()
        {
            _soundEffects.Clear();
            _soundInstancesLoaded = 0;
        }
    }
}
