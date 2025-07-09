using System;
using UnityEngine;

namespace LazyCoder
{
    public static class LCollectHelper
    {
        public static void Spawn(LCollectConfig config, int count, Vector3 spawnPosition, Action onComplete = null)
        {
            // Find destination
            LCollectDestination destination = LCollectDestinationHelper.Get(config);

            // Check destination exist
            if (destination == null)
            {
                LDebug.Log(typeof(LCollectHelper), $"Can't find destination for {config.name}");

                onComplete?.Invoke();

                return;
            }

            // Spawn collect object
            GameObject objCollect = new GameObject(config.name, typeof(RectTransform));

            // Setup collect object
            LCollect collect = objCollect.AddComponent<LCollect>();

            collect.TransformCached.SetParent(destination.TransformCached.parent, false);
            collect.TransformCached.SetAsLastSibling();
            collect.TransformCached.position = spawnPosition;

            collect.Construct(config, destination, count, onComplete);
        }
    }
}