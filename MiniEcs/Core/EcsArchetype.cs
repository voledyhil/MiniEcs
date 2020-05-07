using System.Collections.Generic;

namespace MiniEcs.Core
{
    public class EntityComparer : IEqualityComparer<EcsEntity> 
    {
        public static readonly IEqualityComparer<EcsEntity> Comparer = new EntityComparer();
        public bool Equals(EcsEntity entityA, EcsEntity entityB) 
        {
            return entityA == entityB;
        }

        public int GetHashCode(EcsEntity entity)
        {
            return (int) entity.Id;
        }
    }
    
    public class EcsArchetype
    {
        public int Id { get; }
        public int IndicesCount => Indices.Length;

        public EcsArchetype(int id, byte[] indices)
        {
            Id = id;
            Indices = indices;
        }
        
        public readonly byte[] Indices;
        public readonly HashSet<EcsEntity> Entities = new HashSet<EcsEntity>(EntityComparer.Comparer);
        public readonly Dictionary<byte, EcsArchetype> Next = new Dictionary<byte, EcsArchetype>();
        public readonly Dictionary<byte, EcsArchetype> Prior = new Dictionary<byte, EcsArchetype>();

        public HashSet<EcsEntity>.Enumerator GetEnumerator() 
        {
            return Entities.GetEnumerator();
        }
    }
}