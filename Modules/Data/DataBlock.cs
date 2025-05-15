using System;

namespace LFramework.Data
{
    [System.Serializable]
    public class DataBlock<T> where T : DataBlock<T>
    {
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = DataFileHandler.LoadFromDevice<T>(typeof(T).ToString());

                    if (s_instance == null)
                        s_instance = (T)Activator.CreateInstance(typeof(T));

                    s_instance.Init();
                }

                return s_instance;
            }
        }

        protected virtual void Init()
        {
            MonoCallback.SafeInstance.EventApplicationPause += MonoCallback_ApplicationOnPause;
            MonoCallback.SafeInstance.EventApplicationQuit += MonoCallback_ApplicationOnQuit;
            MonoCallback.SafeInstance.EventApplicationFocus += MonoCAllback_EventApplicationFocus;
        }

        private void MonoCAllback_EventApplicationFocus(bool isFocus)
        {
            if (!isFocus)
                Save();
        }

        private void MonoCallback_ApplicationOnQuit()
        {
            Save();
        }

        private void MonoCallback_ApplicationOnPause(bool paused)
        {
            if (paused)
                Save();
        }

        public static void Save()
        {
            DataFileHandler.SaveToDevice(Instance, typeof(T).ToString());
        }

        public static void Delete()
        {
            s_instance = null;

            DataFileHandler.DeleteInDevice(typeof(T).ToString());
        }
    }
}