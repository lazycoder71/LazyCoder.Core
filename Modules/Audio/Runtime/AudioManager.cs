using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazyCoder.Audio
{
    public static class AudioManager
    {
        public static LValue<float> VolumeMaster = new LValue<float>(1.0f);
        public static LValue<float> VolumeMusic = new LValue<float>(1.0f);
        public static LValue<float> VolumeSound = new LValue<float>(1.0f);

        private static List<AudioPlayer> s_players = new List<AudioPlayer>();

        [RuntimeInitializeOnLoadMethod]
        private static void InitOnStartup()
        {
            VolumeMaster.EventValueChanged += Volume_EventValueChanged;
            VolumeMusic.EventValueChanged += Volume_EventValueChanged;
            VolumeSound.EventValueChanged += Volume_EventValueChanged;
        }

        #region Function -> Private

        private static void Volume_EventValueChanged(float volume)
        {
            for (int i = 0; i < s_players.Count; i++)
            {
                s_players[i].UpdateVolume();
            }
        }

        #endregion

        #region Function -> Public

        public static void Register(AudioPlayer entity)
        {
            s_players.Add(entity);
        }

        public static void Unregister(AudioPlayer entity)
        {
            s_players.Remove(entity);
        }

        public static AudioPlayer Play(AudioConfig config, bool isLoop = false)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            AudioPlayer audioEntity = AudioPlayerPool.Get();

            audioEntity.Play(config, isLoop);

            return audioEntity;
        }

        public static void Stop(AudioConfig config, bool isAll = false)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            for (int i = 0; i < s_players.Count; i++)
            {
                if (s_players[i].Config == config)
                {
                    s_players[i].Stop();

                    if (!isAll)
                        break;
                }
            }
        }

        public static void StopAll()
        {
            for (int i = 0; i < s_players.Count; i++)
            {
                s_players[i].Stop();
            }
        }

        public static AudioPlayer Find(AudioConfig config)
        {
            for (int i = 0; i < s_players.Count; i++)
            {
                if (s_players[i].Config == config)
                {
                    return s_players[i];
                }
            }

            return null;
        }

        #endregion
    }
}