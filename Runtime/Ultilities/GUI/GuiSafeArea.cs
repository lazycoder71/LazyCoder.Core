using Sirenix.OdinInspector;
using UnityEngine;

namespace LazyCoder.Core
{
    /// <summary>
    /// Ensures UI panels respect the device's safe area (e.g., notches, rounded corners).
    /// Attach to the top-level UI element that should conform to the safe area.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class GuiSafeArea : MonoBehaviour
    {
        [Title("Config")]
        [SerializeField] private bool _conformX = true;

        [SerializeField] private bool _conformY = true;

        [Space]
        [SerializeField] private bool _refreshOnUpdate = false;

        [Space]
        [SerializeField] private bool _logging = false;

        private RectTransform _rectTransform = null!;

        private Rect _lastSafeArea = Rect.zero;

        private Vector2Int _lastScreenSize = Vector2Int.zero;

        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        /// <summary>
        /// The RectTransform this component is attached to.
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        private void Awake()
        {
            // Cache RectTransform for performance
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Update()
        {
            if (_refreshOnUpdate)
                Refresh();
        }

        /// <summary>
        /// Refreshes the safe area if screen or orientation has changed.
        /// </summary>
        private void Refresh()
        {
            Rect safeArea = Screen.safeArea;

            if (IsSafeAreaUnchanged(safeArea))
                return;

            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;
            _lastOrientation = Screen.orientation;

            ApplySafeArea(safeArea);
        }

        /// <summary>
        /// Applies the safe area to the RectTransform.
        /// </summary>
        /// <param name="r">The safe area rectangle.</param>
        private void ApplySafeArea(Rect r)
        {
            _lastSafeArea = r;

            if (!_conformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            if (!_conformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            if (Screen.width > 0 && Screen.height > 0)
            {
                Vector2 anchorMin = r.position;
                Vector2 anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                if (IsAnchorValid(anchorMin) && IsAnchorValid(anchorMax))
                {
                    RectTransform.anchorMin = anchorMin;
                    RectTransform.anchorMax = anchorMax;
                }
            }

            if (_logging)
            {
                LDebug.Log<GuiSafeArea>(
                    $"New safe area applied to {name}: x={r.x}, y={r.y}, w={r.width}, h={r.height} on full extents w={Screen.width}, h={Screen.height}");
            }
        }

        /// <summary>
        /// Checks if the safe area and screen state are unchanged.
        /// </summary>
        private bool IsSafeAreaUnchanged(Rect safeArea)
        {
            return safeArea == _lastSafeArea
                   && Screen.width == _lastScreenSize.x
                   && Screen.height == _lastScreenSize.y
                   && Screen.orientation == _lastOrientation;
        }

        /// <summary>
        /// Validates anchor values to avoid NaN or negative values.
        /// </summary>
        private static bool IsAnchorValid(Vector2 anchor)
        {
            return anchor.x >= 0f && anchor.y >= 0f
                                  && !float.IsNaN(anchor.x) && !float.IsNaN(anchor.y);
        }
    }
}