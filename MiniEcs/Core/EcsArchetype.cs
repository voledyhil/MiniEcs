using System.Collections.Generic;

namespace MiniEcs.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Methods to maintain effective comparison of entities for equality.
    /// </summary>
    public class EntityComparer : IEqualityComparer<EcsEntity> 
    {
        public static readonly IEqualityComparer<EcsEntity> Comparer = new EntityComparer();
        public bool Equals(EcsEntity entityA, EcsEntity entityB) 
        {
            return entityA.Id == entityB.Id;
        }

        public int GetHashCode(EcsEntity entity)
        {
            return (int) entity.Id;
        }
    }
    
    /// <summary>
    /// An archetype is a unique combination of component types.
    /// <see cref="EcsWorld"/> uses the archetype to group all objects
    /// that have the same sets of components.
    /// </summary>
    public class EcsArchetype
    {
        /// <summary>
        /// Archetype unique identifier
        /// </summary>
        public int Id { get; }
        
        /// <summary>
        /// Number of unique combinations of component types
        /// </summary>
        public int IndicesCount => Indices.Length;

        /// <summary>
        /// Сreates a new archetype
        /// </summary>
        /// <param name="id">Archetype unique identifier</param>
        /// <param name="indices">Unique combinations of component types</param>
        public EcsArchetype(int id, byte[] indices)
        {
            Id = id;
            Indices = indices;
        }
        
        /// <summary>
        /// Unique combinations of component types
        /// </summary>
        public readonly byte[] Indices;
        
        /// <summary>
        /// Set of entities corresponding to archetype
        /// </summary>
        public readonly HashSet<EcsEntity> Entities = new HashSet<EcsEntity>(EntityComparer.Comparer);
        
        /// <summary>
        /// Transitions to the next archetype when adding a new type of component
        /// </summary>
        public readonly Dictionary<byte, EcsArchetype> Next = new Dictionary<byte, EcsArchetype>();
        
        /// <summary>
        /// Transitions to the previous archetype when deleting an existing component type
        /// </summary>
        public readonly Dictionary<byte, EcsArchetype> Prior = new Dictionary<byte, EcsArchetype>();

        /// <summary>
        /// Returns an enumerator of all entities of a given archetype
        /// </summary>
        /// <returns>Enumerator of entities</returns>
        public HashSet<EcsEntity>.Enumerator GetEnumerator() 
        {
            return Entities.GetEnumerator();
        }
    }
}