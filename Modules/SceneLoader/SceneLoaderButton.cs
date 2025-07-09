using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.SceneLoader
{
    public class SceneLoaderButton : GuiButton
    {
        [System.Serializable]
        private enum Behaviour
        {
            Reload,
            Load,
        }

        [System.Serializable]
        private enum SceneType
        {
            BuildIndex,
            String,
        }

        [SerializeField] private Behaviour _behaviour;

        [HideIf("@_behaviour == Behaviour.Reload")]
        [SerializeField] private SceneType _sceneType;

        [HideIf("@_behaviour == Behaviour.Reload || _sceneType == SceneType.String")]
        [SerializeField] private int _sceneBuildIndex;

        [HideIf("@_behaviour == Behaviour.Reload || _sceneType == SceneType.BuildIndex")]
        [SerializeField] private string _sceneName;

        public override void Button_OnClick()
        {
            base.Button_OnClick();

            switch (_behaviour)
            {
                case Behaviour.Reload:
                    SceneLoaderHelper.Reload();
                    break;

                case Behaviour.Load:
                    LoadScene();
                    break;
            }
        }

        private void LoadScene()
        {
            switch (_sceneType)
            {
                case SceneType.BuildIndex:
                    SceneLoaderHelper.Load(_sceneBuildIndex);
                    break;
                case SceneType.String:
                    SceneLoaderHelper.Load(_sceneName);
                    break;
            }
        }
    }
}
