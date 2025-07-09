using DG.Tweening;
using UnityEngine;

namespace LazyCoder.Audio
{
    public class AudioPlayer : MonoBase
    {
        private AudioConfig _config;

        private AudioSource _audioSource;

        private Tween _tween;
        private Tween _tweenFade;

        private bool _bind;
        private Transform _bindTarget;

        // Internal volume scale, affected by config volume scale and audio settings volume scale
        private float _volume = 1.0f;

        private float _playTime;

        public AudioConfig Config { get { return _config; } }

        public AudioSource AudioSource { get { return _audioSource; } }

        public float PlayTime { get { return _playTime; } }

        #region MonoBehaviour

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            AudioManager.Register(this);
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            _tweenFade?.Kill();

            AudioManager.Unregister(this);
        }

        protected override void LateTick()
        {
            if (!_bind)
                return;

            if (_bindTarget != null)
                TransformCached.position = _bindTarget.position;
            else
                Stop();
        }

        #endregion

        #region Function -> Public

        public void Play(AudioConfig config, bool isLoop = false)
        {
            Construct(config, isLoop);

            _tween?.Kill();

            if (!isLoop)
                _tween = DOVirtual.DelayedCall(config.Clip.length, Stop, false);

            _audioSource.Play();
        }

        public AudioPlayer BindTo(Transform target)
        {
            _bindTarget = target;
            _bind = true;

            return this;
        }

        public AudioPlayer SetPosition(Vector3 position)
        {
            TransformCached.position = position;

            return this;
        }

        public AudioPlayer SetLocalPosition(Vector3 localPosition)
        {
            TransformCached.localPosition = localPosition;

            return this;
        }

        public AudioPlayer SetPitch(float pitch)
        {
            _audioSource.pitch = pitch;

            return this;
        }

        public AudioPlayer FadeIn(float duration, Ease ease = Ease.InSine)
        {
            if (!_audioSource.loop)
                Mathf.Min(duration, _audioSource.clip.length - _audioSource.time);

            _tweenFade?.Kill();
            _tweenFade = DOVirtual.Float(0f, 1.0f, duration, (x) => { _volume = x; UpdateVolume(); })
                                  .SetUpdate(false);

            return this;
        }

        public AudioPlayer FadeOut(float duration, Ease ease = Ease.InSine)
        {
            if (!_audioSource.loop)
                Mathf.Min(duration, _audioSource.clip.length - _audioSource.time);

            _tweenFade?.Kill();
            _tweenFade = DOVirtual.Float(_volume, 0.0f, duration, (x) => { _volume = x; UpdateVolume(); })
                                  .SetUpdate(false)
                                  .OnComplete(Stop);

            return this;
        }

        public void Stop()
        {
            // Prevent calling stop when this audio player already in pool
            if (_config == null)
                return;

            _tween?.Kill();
            _tweenFade?.Kill();

            _audioSource.Stop();

            _config = null;

            AudioPlayerPool.Release(this);
        }

        public void UpdateVolume()
        {
            // Can't update volume if there is no config
            if (_config == null)
                return;

            // Calculate volume final
            float volumeFinal = _volume * _config.VolumeScale * (_config.Type == AudioType.Music ? AudioManager.VolumeMusic.Value : AudioManager.VolumeSound.Value) * AudioManager.VolumeMaster.Value;

            _audioSource.mute = volumeFinal <= 0f;
            _audioSource.volume = volumeFinal;
        }

        #endregion

        #region Function -> Private

        private void Construct(AudioConfig config, bool loop = false)
        {
            // Assign config
            _config = config;

            // Reset bind
            _bind = false;
            _bindTarget = null;

            // Reset volume
            _volume = 1.0f;

            // Setup Audio Source
            _audioSource.clip = config.Clip;
            _audioSource.loop = loop;
            _audioSource.minDistance = config.Distance.x;
            _audioSource.maxDistance = config.Distance.y;
            _audioSource.spatialBlend = config.Is3D ? 1.0f : 0f;
            _audioSource.pitch = config.PitchVariation ? config.PitchVariationRange.RandomWithin() : 1.0f;

            _playTime = Time.time;

            UpdateVolume();
        }

        #endregion
    }
}