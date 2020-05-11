using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    /// <summary>
    /// Query defines the set of component types that an archetype must contain in
    /// order for its entities to be included in the view.
    /// You can also exclude archetypes that contain specific types of components.
    /// </summary>
    public class EcsFilter : IEquatable<EcsFilter>
    {
        public HashSet<byte> Any;
        public HashSet<byte> All;
        public HashSet<byte> None;

        /// <summary>
        /// At least one of the component types in this array must exist in the archetype
        /// </summary>
        /// <param name="types">Combinations of component types</param>
        /// <returns>Current filter</returns>
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

        /// <summary>
        /// All component types in this array must exist in the archetype
        /// </summary>
        /// <param name="types">Combinations of component types</param>
        /// <returns>Current filter</returns>
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

        /// <summary>
        /// None of the component types in this array can exist in the archetype
        /// </summary>
        /// <param name="types">Combinations of component types</param>
        /// <returns>Current filter</returns>
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

        /// <summary>
        /// Creates a new filter with the same set of parameters
        /// </summary>
        /// <returns>New Filter</returns>
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
            if (other == null || other.GetHashCode() != GetHashCode() || other.GetType() != GetType())
                return false;

            if (other.All != null && All != null && !other.All.SetEquals(All))
                return false;

            if (other.Any != null && Any != null && !other.Any.SetEquals(Any))
                return false;

            return other.None == null || None == null || other.None.SetEquals(None);
        }

        private int _hash;
        private bool _isCached;

        /// <summary>
        /// Сalculate a filter hash based on a set of components, for faster filter comparison
        /// </summary>
        /// <returns>Hash</returns>
        public override int GetHashCode()
        {
            if (_isCached)
                return _hash;

            int hash = GetType().GetHashCode();
            hash = CalculateHash(hash, All, 3, 53);
            hash = CalculateHash(hash, Any, 307, 367);
            hash = CalculateHash(hash, None, 647, 683);

            _hash = hash;
            _isCached = true;

            return _hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateHash(int hash, HashSet<byte> indices, int i1, int i2)
        {
            if (indices == null)
                return hash;

            byte[] indicesArray = indices.ToArray();
            Array.Sort(indicesArray);

            hash = indicesArray.Aggregate(hash, (current, index) => current ^ index * i1);
            hash ^= indices.Count * i2;
            return hash;
        }
    }
}