using System;
using System.Collections.Generic;

namespace BinarySerializer.Serializers.Baselines
{
    public interface IBaseline : ICloneable
    {
        new IBaseline Clone();
        bool HasValues { get; }
        void CreateValues(int size);
    }

    public interface IBaseline<TKey> : IBaseline where TKey : unmanaged
    {
        IEnumerable<TKey> BaselineKeys { get; }
        T GetOrCreateBaseline<T>(TKey key, int valuesSize, out bool isNew) where T : class, IBaseline, new();
        int this[byte key] { get; set; }
        void DestroyBaseline(TKey key);
    }

    public class Baseline<TKey> : IBaseline<TKey> where TKey : unmanaged
    {
        public bool HasValues => _values != null;

        public IEnumerable<TKey> BaselineKeys
        {
            get
            {
                if (_baselines == null)
                    _baselines = new Dictionary<TKey, IBaseline>();

                return _baselines.Keys;
            }
        }

        public int this[byte key]
        {
            get => _values[key];
            set => _values[key] = value;
        }

        private Dictionary<TKey, IBaseline> _baselines;
        private int[] _values;

        public Baseline()
        {
        }

        private Baseline(IDictionary<TKey, IBaseline> baselines, int[] values)
        {
            if (values != null)
            {
                _values = new int[values.Length];
                Array.Copy(values, _values, values.Length);
            }

            if (baselines != null)
            {
                _baselines = new Dictionary<TKey, IBaseline>();
                foreach (KeyValuePair<TKey, IBaseline> item in baselines)
                {
                    _baselines.Add(item.Key, item.Value.Clone());
                }
            }
        }

        public T GetOrCreateBaseline<T>(TKey key, int valuesSize, out bool isNew) where T : class, IBaseline, new()
        {
            if (_baselines == null)
                _baselines = new Dictionary<TKey, IBaseline>();

            isNew = false;
            if (_baselines.TryGetValue(key, out IBaseline item))
                return (T) item;

            isNew = true;
            T baseline = new T();
            baseline.CreateValues(valuesSize);

            _baselines.Add(key, baseline);
            return baseline;
        }

        public void DestroyBaseline(TKey key)
        {
            _baselines?.Remove(key);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void CreateValues(int size)
        {
            if (_values != null)
                throw new InvalidOperationException();

            if (size > 0)
                _values = new int[size];
        }

        public IBaseline Clone()
        {
            return new Baseline<TKey>(_baselines, _values);
        } 
    }
}