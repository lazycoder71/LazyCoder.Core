using System;

namespace LFramework.GoogleSheets
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GoogleSheetsAttribute : Attribute
    {
        public string ColumnName { get; }
        public int ColumnIndex { get; }
        public bool IsKey { get; }

        public GoogleSheetsAttribute(string columnName, int columnIndex = -1, bool isKey = false)
        {
            ColumnName = columnName;
            ColumnIndex = columnIndex;
            IsKey = isKey;
        }
    }
}
