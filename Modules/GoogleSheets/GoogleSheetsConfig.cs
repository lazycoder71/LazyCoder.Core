using System;
using UnityEngine;

namespace LFramework.GoogleSheets
{
    [Serializable]
    public class GoogleSheetsConfig 
    {
        [SerializeField] private Color _headerForegroundColor = new Color(0.8941177f, 0.8941177f, 0.8941177f);
        [SerializeField] private Color _headerBackgroundColor = new Color(0.2196079f, 0.2196079f, 0.2196079f);
        [SerializeField] private Color _keyDuplicateColor = new Color(0.8745098f, 0.2240707f, 0.1921569f);

        public Color HeaderForegroundColor => _headerForegroundColor;
        public Color HeaderBackgroundColor => _headerBackgroundColor;
        public Color KeyDuplicateColor => _keyDuplicateColor;
    }
}
