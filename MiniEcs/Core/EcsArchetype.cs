using System.Collections;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsArchetype : IEnumerable<EcsEntity>
    {
    }
    
    public class EcsArchetype : IEcsArchetype
    {        
        public int Id { get; }
        public EcsArchetype(int id)
        {
            Id = id;
        }
        
        public readonly HashSet<byte> Indices = new HashSet<byte>();
        public readonly Dictionary<uint, EcsEntity> Entities = new Dictionary<uint, EcsEntity>();
        public readonly Dictionary<byte, EcsArchetype> Next = new Dictionary<byte, EcsArchetype>();
        public readonly Dictionary<byte, EcsArchetype> Prior = new Dictionary<byte, EcsArchetype>();
        
        public IEnumerator<EcsEntity> GetEnumerator()
        {
            foreach (EcsEntity entity in Entities.Values)
            {
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}