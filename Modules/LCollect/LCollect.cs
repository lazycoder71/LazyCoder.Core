using DG.Tweening;
using LFramework.Pool;
using System;
using UnityEngine;

namespace LFramework
{
    public class LCollect : MonoBase
    {
        LCollectConfig _config;

        LCollectDestination _destination;

        Tween _tween;

        Action _onComplete;

        private void OnDestroy()
        {
            _tween?.Kill();
        }

        public void Construct(LCollectConfig config, LCollectDestination destination, int valueCount, Action onComplete)
        {
            _config = config;
            _destination = destination;
            _onComplete = onComplete;

            // Get spawn count base on input count
            int spawnCount = config.GetSpawnCount(valueCount);

            float delayBetween = spawnCount > 1 ? config.spawnDuration / (spawnCount - 1) : 0.0f;

            // Construct spawn sequence
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition = config.spawnPositions.GetLoop(i);

                sequence.AppendCallback(() => { Spawn(spawnPosition); });
                sequence.AppendInterval(delayBetween);
            }

            sequence.Play();
            sequence.OnComplete(StartCheckEmptyLoop);

            _tween?.Kill();
            _tween = sequence;

            // Notify destination about the return begin
            _destination.CollectBegin(valueCount, spawnCount);
        }

        private void StartCheckEmptyLoop()
        {
            _tween?.Kill();
            _tween = DOVirtual.DelayedCall(1.0f, null, false)
                              .SetLoops(-1, LoopType.Restart)
                              .OnUpdate(CheckEmpty);
        }

        private void CheckEmpty()
        {
            if (TransformCached.childCount == 0)
                Destruct();
        }

        private void Destruct()
        {
            _destination.ReturnEnd();

            _onComplete?.Invoke();

            Destroy(GameObjectCached);
        }

        private void Spawn(Vector3 spawnPosition)
        {
            LCollectItem item = PoolPrefabShared.Get(_config.spawnPrefab, TransformCached).GetComponent<LCollectItem>();

            item.TransformCached.localPosition = spawnPosition;

            item.Construct(_config, _destination);
        }
    }
}
