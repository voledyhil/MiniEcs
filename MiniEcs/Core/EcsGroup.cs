using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsGroup : IEnumerable<EcsEntity>
    {
        
    }
    
    public class EcsGroup : IEcsGroup
    {
        public int Version { get; private set; }
        private readonly List<IEcsArchetype> _archetypes;
        
        public EcsGroup(int version, IEnumerable<IEcsArchetype> archetypes)
        {
            Version = version;
            _archetypes = new List<IEcsArchetype>(archetypes);
        }

        public void Add(int version, IEnumerable<IEcsArchetype> archetypes)
        {
            Version = version;
            _archetypes.AddRange(archetypes);
        }
        
        public IEnumerator<EcsEntity> GetEnumerator()
        {
            foreach (IEcsArchetype archetype in _archetypes)
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