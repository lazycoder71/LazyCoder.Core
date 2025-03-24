using System;
using UnityEngine;

namespace LFramework.LifetimeBinding
{
    public sealed class LifetimeBinding : MonoBehaviour
    {
        private void OnDestroy()
        {
            EventRelease?.Invoke();
        }

        public event Action EventRelease;
    }
}
