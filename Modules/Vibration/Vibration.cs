namespace LazyCoder.Vibration
{
    public static class Vibration
    {
        public static bool Enabled = true;

        public static void Init()
        {
#if UNITY_ANDROID
            VibrationAndroid.Init();
#endif
        }

        public static void Vibrate(VibrationType type)
        {
#if UNITY_IOS && !UNITY_EDITOR
            VibrationIOS.Vibrate(type);
#elif UNITY_ANDROID
            VibrationAndroid.Vibrate(type);
#endif
        }

        public static bool HasVibrator()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return VibrationIOS.HasVibrator();
#elif UNITY_ANDROID
            return VibrationAndroid.HasVibrator();
#else
            return false;
#endif
        }
    }


}