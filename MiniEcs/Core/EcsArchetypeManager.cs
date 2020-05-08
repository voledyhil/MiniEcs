using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsArchetypeManager
    {
        public int ArchetypeCount => _archetypes.Count;
        public EcsArchetype Empty => _emptyArchetype;

        private int _archetypeIdCounter;
        private readonly EcsArchetype _emptyArchetype;
        private readonly List<EcsArchetype> _archetypes;
        private readonly List<EcsArchetype>[] _archetypeIndices;
        
        public EcsArchetypeManager(byte capacity)
        {
            _emptyArchetype = new EcsArchetype(_archetypeIdCounter++, new byte[] { });
            _archetypes = new List<EcsArchetype>(2 * capacity) {_emptyArchetype};
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
            EcsArchetype curArchetype = _emptyArchetype;
            for (int i = 0; i < indices.Length; i++)
            {
                byte index = indices[i];
                if (!curArchetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                {
                    byte[] archetypeIndices = new byte[i + 1];
                    for (int j = 0; j < archetypeIndices.Length; j++)
                        archetypeIndices[j] = indices[j];
                    
                    nextArchetype = new EcsArchetype(_archetypeIdCounter++, archetypeIndices);
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

        public EcsArchetype FindOrCreateNextArchetype(EcsArchetype archetype, byte addIndex)
        {
            if (archetype.Next.TryGetValue(addIndex, out EcsArchetype nextArchetype))
                return nextArchetype;

            bool added = false;
            int length = 0;
            byte[] indices = new byte[archetype.IndicesCount + 1];
            foreach (byte index in archetype.Indices)
            {
                if (addIndex < index && !added)
                {
                    indices[length++] = addIndex;
                    added = true;
                }
                indices[length++] = index;
            }
            if (!added)
                indices[length] = addIndex;
            
            return InnerFindOrCreateArchetype(indices);
        }
        
        public EcsArchetype FindOrCreatePriorArchetype(EcsArchetype archetype, byte removeIndex)
        {
            if (archetype.Prior.TryGetValue(removeIndex, out EcsArchetype priorArchetype))
                return priorArchetype;

            int length = 0;
            byte[] indices = new byte[archetype.IndicesCount - 1];
            foreach (byte index in archetype.Indices)
            {
                if (index != removeIndex)
                    indices[length++] = index;
            }
            return InnerFindOrCreateArchetype(indices);
        }
    }
}