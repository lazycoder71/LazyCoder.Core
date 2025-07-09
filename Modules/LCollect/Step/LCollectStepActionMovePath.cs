using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder
{
    public class LCollectStepActionMovePath : LCollectStepActionMove
    {
        [SerializeField] PathType _pathType;

        [ValidateInput("CheckPoints", "Path Type: Cubic Bezier - Control points must be 2")]
        [SerializeField] Vector3[] _points;

        protected override Tween GetTween(LCollectItem item)
        {
            Vector3 posStart = Vector3.zero;
            Vector3 posEnd = Vector3.zero;

            switch (_journey)
            {
                case Journey.Spawn:
                    posEnd = item.transform.localPosition;
                    posStart = _startAtCenter ? Vector3.zero : posEnd + _startOffset * item.RectTransform.GetUnitPerPixel();
                    break;
                case Journey.Return:
                    posEnd = item.Destination.Position;
                    posStart = item.transform.position;
                    break;
            }

            if (_pathType == PathType.CubicBezier)
            {
                Vector3[] points = new Vector3[3];

                points[0] = posEnd;
                points[1] = posStart + (posEnd - posStart).MultipliedBy(_points[0]);
                points[2] = posStart + (posEnd - posStart).MultipliedBy(_points[1]);

                return item.transform.DOPath(points, _duration, _pathType, PathMode.Sidescroller2D, 10, Color.red);
            }
            else
            {
                Vector3[] points = new Vector3[_points.Length + 2];

                points[0] = posStart;
                points[points.Length - 1] = posEnd;

                for (int i = 0; i < _points.Length; i++)
                {
                    points[i + 1] = posStart + (posEnd - posStart).MultipliedBy(_points[i]);
                }

                return item.transform.DOPath(points, _duration, _pathType, PathMode.Full3D, 10, Color.red);
            }
        }

        private bool CheckPoints()
        {
            if (_pathType == PathType.CubicBezier)
                return _points.Length == 2;
            return true;
        }
    }
}
