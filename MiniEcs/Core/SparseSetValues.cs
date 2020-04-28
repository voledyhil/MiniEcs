 using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class SparseSetValues<T> : IEnumerable<T>
    {
        public ushort Count => _sparseSet.Length;

        private readonly T[] _values;
        private readonly SparseSet _sparseSet;

        public SparseSetValues(ushort capacity)
        {
            _values = new T[capacity];
            _sparseSet = new SparseSet(capacity);
        }
        
        public T this[ushort key]
        {
            get
            {
                if (_sparseSet.Contain(key, out ushort sparse))
                    return _values[sparse];
                
                throw new KeyNotFoundException($"key '{key}' not found");
            }
            set
            {
                _sparseSet.Set(key, out ushort sparse);
                _values[sparse] = value;
            }
        }

        public bool TryGetValue(ushort key, out T value)
        {
            if (_sparseSet.Contain(key, out ushort sparse))
            {
                value = _values[sparse];
                return true;
            }
            value = default;
            return false;
        }
        
        public void Remove(ushort key)
        {
            _sparseSet.UnSet(key, out ushort sparse);
            _values[sparse] = _values[Count];
            _values[Count] = default;
        }

        public bool Contain(ushort key)
        {
            return _sparseSet.Contain(key);
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
                _values[i] = default;

            _sparseSet.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _sparseSet.Length; i++)
            {
                yield return _values[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}