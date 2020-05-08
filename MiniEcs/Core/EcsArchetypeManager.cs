using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsArchetypeManager
    {
        public int ArchetypeCount => _archetypes.Count;
        public EcsArchetype Root => _rootArchetype;

        private int _archetypeCounter;
        private readonly EcsArchetype _rootArchetype;
        private readonly List<EcsArchetype> _archetypes;
        private readonly List<EcsArchetype>[] _archetypeIndices;
        
        public EcsArchetypeManager(byte capacity)
        {
            _rootArchetype = new EcsArchetype(_archetypeCounter++, new byte[] { });
            _archetypes = new List<EcsArchetype>(2 * capacity) {_rootArchetype};
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
                if (archetype.Id <= startId)
                    break;

                yield return archetype;
            }
        }
        
        public EcsArchetype FindOrCreateArchetype(params byte[] indices)
        {
            Array.Sort(indices);

            return InnerFindOrCreateArchetype(indices);
        }

        private EcsArchetype InnerFindOrCreateArchetype(byte[] indices)
        {
            EcsArchetype curArchetype = _rootArchetype;
            for (int i = 0; i < indices.Length; i++)
            {
                byte index = indices[i];
                if (!curArchetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                {
                    byte[] archetypeIndices = new byte[i + 1];
                    for (int j = 0; j < archetypeIndices.Length; j++)
                        archetypeIndices[j] = indices[j];
                    
                    nextArchetype = new EcsArchetype(_archetypeCounter++, archetypeIndices);
                    nextArchetype.Prior[index] = curArchetype;
                    foreach (ushort componentType in nextArchetype.Indices)
                    {
                        _archetypeIndices[componentType].Add(nextArchetype);
                    }

                    curArchetype.Next[index] = nextArchetype;

                    _archetypes.Add(nextArchetype);
                }

                curArchetype = nextArchetype;
            }

            return curArchetype;
        }

        public EcsArchetype FindOrCreateNextArchetype(EcsArchetype archetype, byte index)
        {
            if (archetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                return nextArchetype;

            bool added = false;
            int counter = 0;
            byte[] indices = new byte[archetype.IndicesCount + 1];
            foreach (byte ind in archetype.Indices)
            {
                if (index < ind && !added)
                {
                    indices[counter++] = index;
                    added = true;
                }
                indices[counter++] = ind;
            }
            if (!added)
                indices[counter] = index;
            
            return InnerFindOrCreateArchetype(indices);
        }
        
        public EcsArchetype FindOrCreatePriorArchetype(EcsArchetype archetype, byte index)
        {
            if (archetype.Prior.TryGetValue(index, out EcsArchetype priorArchetype))
                return priorArchetype;

            int counter = 0;
            byte[] indices = new byte[archetype.IndicesCount - 1];
            foreach (byte ind in archetype.Indices)
            {
                if (ind != index)
                    indices[counter++] = ind;
            }
            return InnerFindOrCreateArchetype(indices);
        }
    }
}