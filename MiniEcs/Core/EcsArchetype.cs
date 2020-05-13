using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
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
        /// Number entities in archetype
        /// </summary>
        public int EntitiesCount
        {
            get
            {
                RemoveHoles();
                return _length;
            }
        }
        
        /// <summary>
        /// Unique combinations of component types
        /// </summary>
        public readonly byte[] Indices;

        public readonly HashSet<byte> SetIndices;

        /// <summary>
        /// Transitions to the next archetype when adding a new type of component
        /// </summary>
        public readonly EcsArchetype[] Next = new EcsArchetype[byte.MaxValue];

        /// <summary>
        /// Transitions to the previous archetype when deleting an existing component type
        /// </summary>
        public readonly EcsArchetype[] Prior = new EcsArchetype[byte.MaxValue];
        
        public EcsComponentPool<T> GetComponentPool<T>() where T : IEcsComponent
        {
            return (EcsComponentPool<T>) _componentPools[EcsComponentType<T>.Index];
        } 
        
        public IEcsComponentPool GetComponentPool(byte index)
        {
            return _componentPools[index];
        } 
        
        private int _length;
        private int _count;
        private int _freeIndex = int.MaxValue;
        private EcsEntity[] _entities = new EcsEntity[1];
        private readonly IEcsComponentPool[] _componentPools = new IEcsComponentPool[byte.MaxValue];

        /// <summary>
        /// Сreates a new archetype
        /// </summary>
        /// <param name="id">Archetype unique identifier</param>
        /// <param name="indices">Unique combinations of component types</param>
        public EcsArchetype(int id, byte[] indices)
        {
            Id = id;
            Indices = indices;
            SetIndices = new HashSet<byte>(indices);

            foreach (byte index in indices)
            {
                IEcsComponentPoolCreator creator = EcsComponentTypeManager.ComponentPoolCreators[index];
                _componentPools[index] = creator.InstantiatePool();
            }
        }
        
        /// <summary>
        /// Returns all entities of a given archetype
        /// </summary>
        /// <param name="length">Number entities in archetype</param>
        /// <returns>Entities array</returns>
        public EcsEntity[] GetEntities(out int length)
        {
            RemoveHoles();
            length = _length;
            return _entities;
        }

        /// <summary>
        /// Add Entity to archetype
        /// </summary>
        /// <param name="entity">New Entity</param>
        public void AddEntity(EcsEntity entity)
        {
            if (_length >= _entities.Length)
                Array.Resize(ref _entities, 2 * _entities.Length);

            entity.ArchetypeIndex = _length;
            _entities[_length++] = entity;
            _count++;
        }

        public void AddComponent(byte index, IEcsComponent component)
        {
            _componentPools[index].AddComponent(_length, component);
        }

        /// <summary>
        /// Remove entity from archetype
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveEntity(EcsEntity entity)
        {
            _entities[entity.ArchetypeIndex] = null;
            for (int i = 0; i < IndicesCount; i++)
            {
                _componentPools[Indices[i]].Remove(entity.ArchetypeIndex);
            }
            
            _freeIndex = Math.Min(_freeIndex, entity.ArchetypeIndex);
            _count--;

            if (_freeIndex == _length - 1)
            {
                _length = _freeIndex;
                _freeIndex = int.MaxValue;
            }
            else if (_length >= _count + _count)
            {
                RemoveHoles();
            }
        }

        /// <summary>
        /// Remove holes from the archetype
        /// </summary>
        private void RemoveHoles()
        {
            if (_freeIndex >= _length)
                return;

            int current = _freeIndex + 1;
            while (current < _length)
            {
                while (current < _length && _entities[current] == null)
                    current++;

                if (current >= _length)
                    continue;

                EcsEntity entity = _entities[current];
                entity.ArchetypeIndex = _freeIndex;
                
                _entities[_freeIndex] = entity;
                _entities[current] = null;

                for (int i = 0; i < IndicesCount; i++)
                {
                    _componentPools[Indices[i]].Replace(_freeIndex, current);
                }

                current++;
                _freeIndex++;
            }

            _length = _freeIndex;
            _freeIndex = int.MaxValue;
        }
    }
}