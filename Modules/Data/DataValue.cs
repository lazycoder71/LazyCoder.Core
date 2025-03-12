using MemoryPack;
using System;

namespace LFramework.Data
{
    [MemoryPackable]
    public partial class DataValue<T>
    {
        [MemoryPackInclude] private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                EventValueChanged?.Invoke(_value);
            }
        }

        public event Action<T> EventValueChanged;

        public DataValue(T _value)
        {
            this._value = _value;
        }
    }
}