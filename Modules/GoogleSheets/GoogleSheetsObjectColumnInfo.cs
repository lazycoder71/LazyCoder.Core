using System;
using System.Reflection;

namespace LFramework.GoogleSheets
{
    public class GoogleSheetsObjectColumnInfo
    {
        public string ColumnName { get; }
        public int ColumnIndex { get; }
        public bool IsKey { get; }
        public FieldInfo FieldInfo { get; }
        public PropertyInfo PropertyInfo { get; }
        public Type MemberType { get; }

        public GoogleSheetsObjectColumnInfo(string columnName, int columnIndex, FieldInfo fieldInfo, bool isKey)
        {
            ColumnName = columnName;
            ColumnIndex = columnIndex;
            IsKey = isKey;
            FieldInfo = fieldInfo;
            MemberType = fieldInfo.FieldType;
        }

        public GoogleSheetsObjectColumnInfo(string columnName, int columnIndex, PropertyInfo propertyInfo, bool isKey)
        {
            ColumnName = columnName;
            ColumnIndex = columnIndex;
            IsKey = isKey;
            PropertyInfo = propertyInfo;
            MemberType = propertyInfo.PropertyType;
        }

        public object GetValue(object obj)
        {
            if (FieldInfo != null)
                return FieldInfo.GetValue(obj);
            else if (PropertyInfo != null)
                return PropertyInfo.GetValue(obj);
            return null;
        }

        public void SetValue(object obj, object value)
        {
            try
            {
                var convertedValue = ConvertValue(value?.ToString(), MemberType);

                if (FieldInfo != null)
                    FieldInfo.SetValue(obj, convertedValue);
                else if (PropertyInfo != null && PropertyInfo.CanWrite)
                    PropertyInfo.SetValue(obj, convertedValue);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to set value for {ColumnName}: {ex.Message}");
            }
        }

        private object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return GetDefaultValue(targetType);

            try
            {
                // Handle nullable types first
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlyingType = Nullable.GetUnderlyingType(targetType);
                    return ConvertValue(value, underlyingType);
                }

                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(int))
                    return int.Parse(value);

                if (targetType == typeof(float))
                    return float.Parse(value);

                if (targetType == typeof(double))
                    return double.Parse(value);

                if (targetType == typeof(decimal))
                    return decimal.Parse(value);

                if (targetType == typeof(bool))
                    return bool.Parse(value);

                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);

                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Failed to convert '{value}' to {targetType.Name}: {ex.Message}");
                return GetDefaultValue(targetType);
            }
        }

        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
