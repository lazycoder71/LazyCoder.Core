using System;
using UnityEngine;

namespace LazyCoder.Data
{
#if LFRAMEWORK_MEMORYPACK
    [MemoryPack.MemoryPackable]
#endif
    public partial class DataValue<T>
    {
#if LFRAMEWORK_MEMORYPACK
        [MemoryPack.MemoryPackInclude]
#endif
        [SerializeField] private T _value;

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