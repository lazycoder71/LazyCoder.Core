using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace LazyCoder
{
    public class LCollectDestination : MonoBase
    {
        [Title("Reference")]
        [SerializeField] private Transform _target;

        [Title("Config")]
        [SerializeField] private LCollectConfig _config;

        public event Action<int, int> EventCollectBegin;
        public event Action<float> EventCollect;
        public event Action EventCollectEnd;

        int _returnExpect;
        int _returnCount;

        public LCollectConfig Config { get { return _config; } }

        public Vector3 Position { get { return _target == null ? TransformCached.position : _target.position; } }

        protected override void OnEnable()
        {
            base.OnEnable();

            LCollectDestinationHelper.Push(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LCollectDestinationHelper.Pop(this);
        }

        public void CollectBegin(int valueCount, int spawnCount)
        {
            _returnExpect += spawnCount;

            EventCollectBegin?.Invoke(valueCount, spawnCount);
        }

        public void Collect()
        {
            _returnCount++;

            if (_returnCount == _returnExpect)
            {
                _returnCount = 0;
                _returnExpect = 0;

                EventCollect?.Invoke(1f);
            }
            else
            {
                EventCollect?.Invoke((float)_returnCount / _returnExpect);
            }
        }

        public void ReturnEnd()
        {
            EventCollectEnd?.Invoke();
        }
    }
}