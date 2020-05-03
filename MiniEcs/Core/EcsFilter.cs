using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsFilter
    {
        public HashSet<byte> Any;
        public HashSet<byte> All;
        public HashSet<byte> None;
        
        public EcsFilter AnyOf(params byte[] types)
        {
            Any = Any ?? new HashSet<byte>(); 
            foreach (byte type in types)
            {
                Any.Add(type);
                _isCached = false;
            }
            return this;
        }

        public EcsFilter AllOf(params byte[] types)
        {
            All = All ?? new HashSet<byte>(); 
            foreach (byte type in types)
            {
                All.Add(type);
                _isCached = false;
            }
            return this;
        }

        public EcsFilter NoneOf(params byte[] types)
        {
            None = None ?? new HashSet<byte>(); 
            foreach (byte type in types)
            {
                None.Add(type);
                _isCached = false;
            }
            return this;
        }

        public EcsFilter Clone()
        {
            EcsFilter filter = new EcsFilter();

            if (Any != null)
                filter.Any = new HashSet<byte>(Any);

            if (All != null)
                filter.All = new HashSet<byte>(All);

            if (None != null)
                filter.None = new HashSet<byte>(None);

            return filter;
        }
        
        
        public override bool Equals(object obj) 
        {
            if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode()) 
                return false;

            EcsFilter filter = (EcsFilter)obj;
            
            if (filter.All != null && All != null && !filter.All.SetEquals(All))
                return false;

            if (filter.Any != null && Any != null && !filter.Any.SetEquals(Any))
                return false;

            return filter.None == null || None == null || filter.None.SetEquals(None);
        }


        private int _hash;
        private bool _isCached;

        public override int GetHashCode()
        {
            if (_isCached) 
                return _hash;
            
            int hash = GetType().GetHashCode();
            hash = ApplyHash(hash, All, 3, 53);
            hash = ApplyHash(hash, Any, 307, 367);
            hash = ApplyHash(hash, None, 647, 683);
            
            _hash = hash;
            _isCached = true;

            return _hash;
        }

        private static int ApplyHash(int hash, HashSet<byte> indices, int i1, int i2)
        {
            if (indices == null)
                return hash;

            foreach (byte index in indices)
            {
                hash ^= index * i1;
            }

            hash ^= indices.Count * i2;
            return hash;
        }
    }
}