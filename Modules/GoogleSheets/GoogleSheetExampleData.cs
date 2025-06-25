using UnityEngine;

namespace LFramework.GoogleSheets
{
    [System.Serializable]
    public class GoogleSheetExampleData : ScriptableObject
    {
        [GoogleSheets("ID", 0, isKey: true)]
        public string id { get { return this.name; } }

        [GoogleSheets("Name")]
        public string playerName;

        [GoogleSheets("Level")]
        public int level;

        [GoogleSheets("Score")]
        public float score;

        [GoogleSheets("IsActive")]
        public bool isActive;
    }
}
