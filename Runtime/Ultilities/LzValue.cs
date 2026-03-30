using System;
using MemoryPack;

namespace LazyCoder.Core
{
    [MemoryPackable]
    public partial class LzValue<T>
    {
        [MemoryPackInclude] private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value))
                    return;

                T oldValue = _value;

                _value = value;

                EventValueChanged?.Invoke(oldValue, value);
            }
        }

        /// <summary>
        /// Value changed event, first argument is the old value, second is the new value
        /// </summary>
        public event Action<T, T> EventValueChanged;

        public LzValue(T _value)
        {
            this._value = _value;
        }
    }
}