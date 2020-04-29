using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsArchetype
    {
        IEnumerable<byte> ArchetypeIndices { get; }
        IEnumerable<EcsEntity> ArchetypeEntities { get; }
        int EntitiesCount { get; }
    }
}