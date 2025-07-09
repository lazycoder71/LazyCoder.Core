#if UNITY_ANDROID

using UnityEngine;

namespace LazyCoder.Vibration
{
    /// <summary>
    /// Class for controlling Vibration on Android. Automatically initializes before scene is loaded.
    /// Original file: https://gist.github.com/ruzrobert/d98220a3b7f71ccc90403e041967c46b
    /// Edited by lazycoder71
    /// </summary>
    public static class VibrationAndroid
    {
        private static readonly long _lightDuration = 20;
        public static readonly long _mediumDuration = 40;
        public static readonly long _heavyDuration = 80;

        public static readonly int _lightAmplitude = 40;
        public static readonly int _mediumAmplitude = 120;
        public static readonly int _heavyAmplitude = 255;

        private static readonly long[] _successPattern = { 0, _lightDuration, _lightDuration, _heavyDuration };
        private static readonly int[] _successPatternAmplitude = { 0, _lightAmplitude, 0, _heavyAmplitude };

        private static readonly long[] _warningPattern = { 0, _heavyDuration, _lightDuration, _mediumDuration };
        private static readonly int[] _warningPatternAmplitude = { 0, _heavyAmplitude, 0, _mediumAmplitude };

        private static readonly long[] _failurePattern = { 0, _mediumDuration, _lightDuration, _mediumDuration, _lightDuration, _heavyDuration, _lightDuration, _lightDuration };
        private static readonly int[] _failurePatternAmplitude = { 0, _mediumAmplitude, 0, _mediumAmplitude, 0, _heavyAmplitude, 0, _lightAmplitude };

        // Initialization flag
        private static bool _isInitialized = false;

        // Vibrator references
        private static AndroidJavaObject _vibrator = null;
        private static AndroidJavaClass _vibrationEffectClass = null;
        private static int _defaultAmplitude = 255;

        // Api level
        private static int _apiLevel = 1;

        // Available only from Api >= 26
        private static bool _isSupportVibrationEffect { get { return _apiLevel >= 26; } }

        #region Initialization

        public static void Init()
        {
            if (_isInitialized)
                return;

            if (Application.platform != RuntimePlatform.Android)
                return;

            // Get Api Level
            using (AndroidJavaClass androidVersionClass = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                _apiLevel = androidVersionClass.GetStatic<int>("SDK_INT");
            }

            // Get UnityPlayer and CurrentActivity
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (currentActivity != null)
                {
                    _vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                    // If device supports vibration effects, get corresponding class
                    if (_isSupportVibrationEffect)
                    {
                        _vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                        _defaultAmplitude = Mathf.Clamp(_vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE"), 1, 255);
                    }
                }
            }

            LDebug.Log(typeof(VibrationAndroid), $"Initialized" +
                $"\nDevice has Vibrator = {HasVibrator()}" +
                $"\nDevice support Amplitude Control = {HasAmplitudeControl()}" +
                $"\nDefault amplitude = {_defaultAmplitude}");

            _isInitialized = true;
        }

        #endregion

        #region Functions -> Public

        public static void Vibrate(VibrationType type)
        {
            switch (type)
            {
                case VibrationType.Default:
                    Handheld.Vibrate();
                    break;

                case VibrationType.ImpactLight:
                    Vibrate(_lightDuration, _lightAmplitude);
                    break;

                case VibrationType.ImpactMedium:
                    Vibrate(_mediumDuration, _mediumAmplitude);
                    break;

                case VibrationType.ImpactHeavy:
                    Vibrate(_heavyDuration, _heavyAmplitude);
                    break;

                case VibrationType.Success:
                    Vibrate(_successPattern, _successPatternAmplitude);
                    break;

                case VibrationType.Failure:
                    Vibrate(_failurePattern, _failurePatternAmplitude);
                    break;

                case VibrationType.Warning:
                    Vibrate(_warningPattern, _warningPatternAmplitude);
                    break;

                default:
                    LDebug.Log(typeof(VibrationAndroid), $"Undefined vibration type {type}");
                    break;
            }
        }

        /// <summary>
        /// Vibrate for Milliseconds, with Amplitude (if available).
        /// If amplitude is -1, amplitude is Disabled. If -1, device DefaultAmplitude is used. Otherwise, values between 1-255 are allowed.
        /// If 'cancel' is true, Cancel() will be called automatically.
        /// </summary>
        public static void Vibrate(long milliseconds, int amplitude = 0, bool cancel = false)
        {
            // Lazy initialize
            Init();

            if (!HasVibrator())
                return;

            if (cancel)
                Cancel();

            if (_isSupportVibrationEffect)
            {
                // Validate amplitude
                amplitude = Mathf.Clamp(amplitude, -1, 255);

                // If less -1 or don't have amplitude control, disable amplitude (use maximum amplitude)
                if (amplitude <= -1 || !HasAmplitudeControl())
                    amplitude = 255;

                // If 0, use device DefaultAmplitude
                if (amplitude == 0)
                    amplitude = _defaultAmplitude;

                VibrateEffect(milliseconds, amplitude);
            }
            else
            {
                VibrateLegacy(milliseconds);
            }
        }

        /// <summary>
        /// Vibrate Pattern (pattern of durations, with format Off-On-Off-On and so on).
        /// Amplitudes can be Null (for default) or array of Pattern array length with values between 1-255.
        /// To cause the pattern to repeat, pass the index into the pattern array at which to start the repeat, or -1 to disable repeating.
        /// If 'cancel' is true, Cancel() will be called automatically.
        /// </summary>
        public static void Vibrate(long[] pattern, int[] amplitudes = null, int repeat = -1, bool cancel = false)
        {
            // Lazy initialize
            Init();

            if (!HasVibrator())
                return;

            if (!HasAmplitudeControl())
                amplitudes = null;

            // Check Amplitudes array length
            if (amplitudes != null && amplitudes.Length != pattern.Length)
            {
                LDebug.LogWarning(typeof(VibrationAndroid), "Length of Amplitudes array is not equal to Pattern array. Amplitudes will be ignored.");

                amplitudes = null;
            }

            // Limit amplitudes between 1 and 255
            if (amplitudes != null)
                ClampAmplitudesArray(amplitudes);

            if (cancel)
                Cancel();

            if (_isSupportVibrationEffect)
            {
                if (amplitudes != null)
                    VibrateEffect(pattern, amplitudes, repeat);
                else
                    VibrateEffect(pattern, repeat);
            }
            else
            {
                VibrateLegacy(pattern, repeat);
            }
        }

        /// <summary>
        /// Returns true if device has vibrator
        /// </summary>
        public static bool HasVibrator()
        {
            return _vibrator != null && _vibrator.Call<bool>("hasVibrator");
        }

        /// <summary>
        /// Return true if device supports amplitude control
        /// </summary>
        public static bool HasAmplitudeControl()
        {
            if (HasVibrator() && _isSupportVibrationEffect)
                return _vibrator.Call<bool>("hasAmplitudeControl"); // API 26+ specific
            else
                return false; // no amplitude control below API level 26
        }

        /// <summary>
        /// Tries to cancel current vibration
        /// </summary>
        public static void Cancel()
        {
            if (HasVibrator())
                _vibrator.Call("cancel");
        }

        #endregion

        #region Function -> Private

        private static void VibrateEffect(long milliseconds, int amplitude)
        {
            using (AndroidJavaObject effect = CreateEffect_OneShot(milliseconds, amplitude))
            {
                _vibrator.Call("vibrate", effect);
            }
        }

        private static void VibrateLegacy(long milliseconds)
        {
            _vibrator.Call("vibrate", milliseconds);
        }

        private static void VibrateEffect(long[] pattern, int repeat)
        {
            using (AndroidJavaObject effect = CreateEffect_Waveform(pattern, repeat))
            {
                _vibrator.Call("vibrate", effect);
            }
        }

        private static void VibrateLegacy(long[] pattern, int repeat)
        {
            _vibrator.Call("vibrate", pattern, repeat);
        }

        private static void VibrateEffect(long[] pattern, int[] amplitudes, int repeat)
        {
            using (AndroidJavaObject effect = CreateEffect_Waveform(pattern, amplitudes, repeat))
            {
                _vibrator.Call("vibrate", effect);
            }
        }

        private static void VibrateEffectPredefined(int effectId)
        {
            using (AndroidJavaObject effect = CreateEffect_Predefined(effectId))
            {
                _vibrator.Call("vibrate", effect);
            }
        }

        /// <summary>
        /// Wrapper for public static VibrationEffect createOneShot (long milliseconds, int amplitude). API >= 26
        /// </summary>
        private static AndroidJavaObject CreateEffect_OneShot(long milliseconds, int amplitude)
        {
            return _vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
        }

        /// <summary>
        /// Wrapper for public static VibrationEffect createPredefined (int effectId). API >= 29
        /// </summary>
        private static AndroidJavaObject CreateEffect_Predefined(int effectId)
        {
            return _vibrationEffectClass.CallStatic<AndroidJavaObject>("createPredefined", effectId);
        }

        /// <summary>
        /// Wrapper for public static VibrationEffect createWaveform (long[] timings, int[] amplitudes, int repeat). API >= 26
        /// </summary>
        private static AndroidJavaObject CreateEffect_Waveform(long[] timings, int[] amplitudes, int repeat)
        {
            return _vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, amplitudes, repeat);
        }

        /// <summary>
        /// Wrapper for public static VibrationEffect createWaveform (long[] timings, int repeat). API >= 26
        /// </summary>
        private static AndroidJavaObject CreateEffect_Waveform(long[] timings, int repeat)
        {
            return _vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", timings, repeat);
        }

        private static void ClampAmplitudesArray(int[] amplitudes)
        {
            for (int i = 0; i < amplitudes.Length; i++)
            {
                amplitudes[i] = Mathf.Clamp(amplitudes[i], 1, 255);
            }
        }

        #endregion
    }
}

#endif