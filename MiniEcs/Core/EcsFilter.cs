using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    public class EcsFilter : IEquatable<EcsFilter>
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

        public bool Equals(EcsFilter other)
        {
            if (other == null || other.GetType() != GetType() || other.GetHashCode() != GetHashCode())
                return false;

            if (other.All != null && All != null && !other.All.SetEquals(All))
                return false;

            if (other.Any != null && Any != null && !other.Any.SetEquals(Any))
                return false;

            return other.None == null || None == null || other.None.SetEquals(None);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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