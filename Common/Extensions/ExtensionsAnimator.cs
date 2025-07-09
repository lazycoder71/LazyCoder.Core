using UnityEngine;

namespace LazyCoder
{
    public static class ExtensionsAnimator
    {
        public static float GetLength(this Animator animator, string clipName)
        {
            var controller = animator.runtimeAnimatorController;
            var clips = controller.animationClips;
            int count = clips.Length;

            for (int i = 0; i < count; i++)
            {
                var clip = clips[i];
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }

            LDebug.Log(typeof(ExtensionsAnimator), $"Get clip length failed: Clip {clipName} doesn't exist!");

            return 0f;
        }
    }
}
