using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class SparseSet : IEnumerable<ushort>
    {
        public ushort Length { get; private set; }

        private readonly int _capacity;
        private readonly ushort[] _dense;
        private readonly ushort[] _sparse;

        public ushort this[int index] => _dense[index];

        public SparseSet(int capacity)
        {
            _capacity = capacity;
            
            _dense = new ushort[capacity];
            _sparse = new ushort[capacity];
        }

        public void Set(ushort value, out ushort sparse)
        {
            CheckKey(value);

            sparse = _sparse[value];
            if (sparse < Length && _dense[sparse] == value) 
                return;
            
            sparse = Length++;
            _sparse[value] = sparse;
            _dense[sparse] = value;
        }
        
        public void UnSet(ushort value, out ushort sparse)
        {
            CheckKey(value);

            sparse = _sparse[value];
            if (sparse >= Length || _dense[sparse] != value)
                return;

            ushort dense = _dense[--Length];
            _dense[sparse] = dense;  
            _sparse[dense] = sparse;
        }
                
        public bool Contain(ushort value, out ushort sparse)
        {
            CheckKey(value);
            
            sparse = _sparse[value];
            return sparse < Length && _dense[sparse] == value;            
        }
        
        public void Set(SparseSet set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                Set(set._dense[i]);
            }
        }
        
        public void UnSet(SparseSet set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                UnSet(set._dense[i]);
            }
        }

        public void Set(ushort value)
        {
            Set(value, out _);
        }

        public void UnSet(ushort value)
        {
            UnSet(value, out _);
        }

        public bool Contain(ushort value)
        {
            return Contain(value, out _);
        }
        
        public void Sort()
        {
            Array.Sort(_dense, 0, Length);

            for (ushort i = 0; i < Length; i++)
            {
                _sparse[_dense[i]] = i;
            }
        }

        public void Clear()
        {
            Length = 0;
        }
               
        private void CheckKey(ushort value)
        {
            if (value >= _capacity)
                throw new ArgumentOutOfRangeException($"Key was out of range. Must be less than the size '{value}'");
        }
        
        public IEnumerator<ushort> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return _dense[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}