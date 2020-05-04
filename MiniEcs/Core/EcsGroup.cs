using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsGroup : IEnumerable<EcsEntity>
    {
        EcsEntity this[uint id] { get; }
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
        
        public EcsEntity this[uint id]
        {
            get
            {
                foreach (EcsArchetype archetype in _archetypes)
                {
                    if (archetype.TryGetEntity(id, out EcsEntity entity))
                        return entity;
                }
                throw new KeyNotFoundException();
            }
        }
        
        public IEnumerator<EcsEntity> GetEnumerator()
        {
            foreach (EcsArchetype archetype in _archetypes)
            {
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