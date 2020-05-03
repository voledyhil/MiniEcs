using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsArchetypeManager
    {
        public EcsArchetype RootArchetype { get; }
        public int ArchetypeCount => _archetypes.Count;

        private int _archetypeCounter = 0;
        private readonly List<EcsArchetype> _archetypes;
        private readonly List<EcsArchetype>[] _archetypeIndices;
        
        public EcsArchetypeManager(byte capacity)
        {
            RootArchetype = new EcsArchetype(_archetypeCounter++);
            
            _archetypes = new List<EcsArchetype>(2 * capacity) {RootArchetype};
            _archetypeIndices = new List<EcsArchetype>[capacity];
            
            for (int i = 0; i < capacity; i++)
            {
                _archetypeIndices[i] = new List<EcsArchetype>(capacity);
            }
        }

        public IEnumerable<EcsArchetype> GetArchetypes(int startId)
        {
            for (int i = startId; i < _archetypes.Count; i++)
            {
                yield return _archetypes[i];
            }
        }
        
        public IEnumerable<EcsArchetype> GetArchetypes(byte index, int startId)
        {
            List<EcsArchetype> archetypes = _archetypeIndices[index];
            
            for (int i = archetypes.Count - 1; i >= 0; i--)
            {
                EcsArchetype archetype = archetypes[i];
                if (archetype.Id < startId)
                    break;

                yield return archetype;
            }
        }
        
        public EcsArchetype FindOrCreateArchetype(byte[] indices)
        {
            Array.Sort(indices);

            EcsArchetype curArchetype = RootArchetype;
            foreach (byte index in indices)
            {
                if (!curArchetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                {
                    nextArchetype = new EcsArchetype(_archetypeCounter++);
                    nextArchetype.Indices.UnionWith(curArchetype.Indices);
                    nextArchetype.Indices.Add(index);
                    nextArchetype.Prior[index] = curArchetype;

                    foreach (ushort componentType in nextArchetype.Indices)
                    {
                        _archetypeIndices[componentType].Add(nextArchetype);
                    }
                   
                    _archetypes.Add(nextArchetype);

                    curArchetype.Next[index] = nextArchetype;
                }

                curArchetype = nextArchetype;
            }

            return curArchetype;
        }
    }
}