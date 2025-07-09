using System;
using UnityEngine;

namespace LazyCoder.LifetimeBinding
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
