using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsArchetypeManager
    {
        public EcsArchetype RootArchetype { get; }

        private readonly List<EcsArchetype> _allArchetypes;
        private readonly List<EcsArchetype>[] _archetypeIndices;
        
        public EcsArchetypeManager(byte capacity)
        {
            RootArchetype = new EcsArchetype();
            
            _allArchetypes = new List<EcsArchetype>(2 * capacity) {RootArchetype};
            _archetypeIndices = new List<EcsArchetype>[capacity];
            
            for (int i = 0; i < capacity; i++)
            {
                _archetypeIndices[i] = new List<EcsArchetype>(capacity);
            }
        }

        public IReadOnlyList<EcsArchetype> AllArchetypes => _allArchetypes;
        public IReadOnlyList<EcsArchetype> this[byte index] => _archetypeIndices[index];

        public EcsArchetype FindOrCreateArchetype(byte[] indices)
        {
            Array.Sort(indices);

            EcsArchetype curArchetype = RootArchetype;
            foreach (byte index in indices)
            {
                if (!curArchetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                {
                    nextArchetype = new EcsArchetype();
                    nextArchetype.Indices.UnionWith(curArchetype.Indices);
                    nextArchetype.Indices.Add(index);
                    nextArchetype.Prior[index] = curArchetype;

                    foreach (ushort componentType in nextArchetype.Indices)
                    {
                        _archetypeIndices[componentType].Add(nextArchetype);
                    }
                   
                    _allArchetypes.Add(nextArchetype);

                    curArchetype.Next[index] = nextArchetype;
                }

                curArchetype = nextArchetype;
            }

            return curArchetype;
        }
    }
}