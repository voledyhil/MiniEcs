using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsGroup : IEnumerable<EcsEntity>
    {
        int CalculateCount();
    }
    
    public class EcsGroup : IEcsGroup
    {
        public int Version { get; private set; }
        private readonly List<EcsArchetype> _archetypes;
        
        public EcsGroup(int version, IEnumerable<EcsArchetype> archetypes)
        {
            Version = version;
            _archetypes = new List<EcsArchetype>(archetypes);
        }

        public void IncVersion(int newVersion, IEnumerable<EcsArchetype> newArchetypes)
        {
            Version = newVersion;
            _archetypes.AddRange(newArchetypes);
        }

        public int CalculateCount()
        {
            int count = 0;
            foreach (EcsArchetype archetype in _archetypes)
            {
                count += archetype.Entities.Count;
            }
            return count;
        }

        public IEnumerator<EcsEntity> GetEnumerator()
        {
            for (int i = 0; i < _archetypes.Count; i++)
            {
                EcsArchetype archetype = _archetypes[i];
                
                foreach (EcsEntity entity in archetype)
                {
                    yield return entity;
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}