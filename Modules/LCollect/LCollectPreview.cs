using Sirenix.OdinInspector;
using UnityEngine;
using Vertx.Debugging;

namespace LazyCoder
{
    [RequireComponent(typeof(RectTransform))]
    public class LCollectPreview : MonoBehaviour
    {
        [Title("Reference")]
        [SerializeField, AssetsOnly] LCollectConfig _config;
        [SerializeField] Color _colorStart = Color.green;
        [SerializeField] Color _colorEnd = Color.red;

        private void OnDrawGizmos()
        {
            if (_config == null)
                return;

            float unitPerPixel = GetComponent<RectTransform>().GetUnitPerPixel();

            for (int i = 0; i < _config.spawnPositions.Count; i++)
            {
                D.raw(new Shape.Circle2D(transform.TransformPoint(_config.spawnPositions[i]), 5.0f * unitPerPixel), Color.Lerp(_colorStart, _colorEnd, (float)i / (_config.spawnPositions.Count - 1)));
            }

            D.raw(new Shape.Circle2D(transform.position, _config.spawnSampleRadius * unitPerPixel), Color.yellow);
        }
    }
}
