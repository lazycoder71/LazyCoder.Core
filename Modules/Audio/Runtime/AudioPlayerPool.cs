using UnityEngine;
using UnityEngine.Pool;

namespace LazyCoder.Audio
{
    public class AudioPlayerPool
    {
        private static ObjectPool<AudioPlayer> s_pool;

        #region Function -> Public

        public static void Release(AudioPlayer audioScript)
        {
            s_pool.Release(audioScript);
        }

        public static AudioPlayer Get()
        {
            InitPool();

            AudioPlayer audio = null;

            while (audio == null)
                audio = s_pool.Get();

            return audio;
        }

        #endregion

        #region Function -> Private

        private static void InitPool()
        {
            if (s_pool != null)
                return;

            s_pool = new ObjectPool<AudioPlayer>(
                () =>
                {
                    // Create object with audio script + audio source
                    AudioPlayer audio = new GameObject(typeof(AudioPlayer).ToString(), typeof(AudioSource)).AddComponent<AudioPlayer>();

                    // Audio should not stop when scene changed
                    Object.DontDestroyOnLoad(audio.GameObjectCached);

                    return audio;
                },
                (audio) => { if (audio != null) audio.GameObjectCached.SetActive(true); },
                (audio) => { if (audio != null) audio.GameObjectCached.SetActive(false); },
                (audio) => { },
#if UNITY_EDITOR
                true); // Keep heavy check on editor
#else
                false);
#endif
        }

        #endregion
    }
}