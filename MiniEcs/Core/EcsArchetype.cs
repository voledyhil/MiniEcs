using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EcsArchetype : IEcsArchetype
    {
        public IEnumerable<byte> ArchetypeIndices => Indices;
        public IEnumerable<EcsEntity> ArchetypeEntities => Entities.Values;
        public int EntitiesCount => Entities.Count;

        public readonly HashSet<byte> Indices = new HashSet<byte>();
        public readonly Dictionary<uint, EcsEntity> Entities = new Dictionary<uint, EcsEntity>();
        public readonly Dictionary<byte, EcsArchetype> Next = new Dictionary<byte, EcsArchetype>();
        public readonly Dictionary<byte, EcsArchetype> Prior = new Dictionary<byte, EcsArchetype>(); 
    }
}