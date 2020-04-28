using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class Map<T1, T2>
    {
        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }

        public int Count => Forward.Count;
        
        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            _reverse.Remove(_forward[t1]);
            _forward.Remove(t1);
        }
        
        public void Remove(T2 t2)
        {
            _forward.Remove(_reverse[t2]);
            _reverse.Remove(t2);
        }

        
        public bool TryGetValue(T1 t1, out T2 t2)
        {
            return _forward.TryGetValue(t1, out t2);
        }
        
        public bool TryGetValue(T2 t2, out T1 t1)
        {
            return _reverse.TryGetValue(t2, out t1);
        }

        public class Indexer<T3, T4>
        {
            public int Count => _dictionary.Count;
            
            private readonly Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            
            public T4 this[T3 index] => _dictionary[index];
        }
    }
}