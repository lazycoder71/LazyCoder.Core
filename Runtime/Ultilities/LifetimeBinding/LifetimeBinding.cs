using System;
using UnityEngine;

namespace LazyCoder.Core
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
