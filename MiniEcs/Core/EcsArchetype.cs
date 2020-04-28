using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsArchetype : IEnumerable<uint>
    {
        int EntitiesCount { get; }
    }
    
    public class EcsArchetype : IEcsArchetype
    {
        public ushort Id { get; }
        public int EntitiesCount => Entities.Count;
        
        public readonly SparseSet Types = new SparseSet(EcsWorld.MAX_COMPONENT_TYPE_COUNT);
        public readonly SparseSetValues<uint> Entities = new SparseSetValues<uint>(EcsWorld.MAX_ENTITY_COUNT);
        public readonly SparseSetValues<EcsArchetype> Next = new SparseSetValues<EcsArchetype>(EcsWorld.MAX_ARCHETYPE_COUNT);
        public readonly SparseSetValues<EcsArchetype> Prior = new SparseSetValues<EcsArchetype>(EcsWorld.MAX_ARCHETYPE_COUNT);

        public EcsArchetype(ushort id)
        {
            Id = id;
        }
        
        public IEnumerator<uint> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}