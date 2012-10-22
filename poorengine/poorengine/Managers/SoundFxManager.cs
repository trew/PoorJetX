using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using PoorEngine.Settings;

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

        public static void SetVolume(string category, float volume)
        {
            if (category == "Global")
                GameSettings.Default.GlobalSoundVolume = MathHelper.Clamp(volume, 0, 1);
            if (category == "Music")
                GameSettings.Default.MusicVolume = MathHelper.Clamp(volume, 0, 1);
            if (category == "Sound")
                GameSettings.Default.SoundVolume = MathHelper.Clamp(volume, 0, 1);

            MediaPlayer.Volume = GameSettings.Default.MusicVolume * GameSettings.Default.GlobalSoundVolume;

        }


        /// <summary>
        /// Categories is either "Sound" or "Music". It returns the `actual`
        /// sound value, modified by Global sound volume.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static float GetVolume(string category, float volume)
        {
            if (category == "Sound")
                return GameSettings.Default.SoundVolume * GameSettings.Default.GlobalSoundVolume * volume;
            if (category == "Music")
                return GameSettings.Default.MusicVolume * GameSettings.Default.GlobalSoundVolume * volume;
            return 0;
        }

        public static void RemoveFx(int id)
        {

            if (_soundEffects.ContainsKey(id))
            {
                _soundEffects[id].Stop();
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
                if(i.State == SoundState.Playing)
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
