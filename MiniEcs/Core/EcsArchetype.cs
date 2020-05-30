using System;
using System.Collections.Generic;

namespace MiniEcs.Core
{
    public interface IEcsArchetype
    {
        int Id { get; }
        
        /// <summary>
        /// Unique combinations of component types
        /// </summary>
        byte[] Indices { get; }

        int EntitiesCount { get; }
        IEcsEntity this[int index] { get; }
        IEcsComponentPool GetComponentPool(byte index);
    }

    /// <summary>
    /// An archetype is a unique combination of component types.
    /// <see cref="EcsWorld"/> uses the archetype to group all objects
    /// that have the same sets of components.
    /// </summary>
    public class EcsArchetype : IEcsArchetype
    {
        public int Id { get; }
        
        /// <inheritdoc />
        /// <summary>
        /// Unique combinations of component types
        /// </summary>
        public byte[] Indices { get; }

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
        public readonly HashSet<byte> SetIndices;

        /// <summary>
        /// Transitions to the next archetype when adding a new type of component
        /// </summary>
        public readonly EcsArchetype[] Next = new EcsArchetype[byte.MaxValue];

        /// <summary>
        /// Transitions to the previous archetype when deleting an existing component type
        /// </summary>
        public readonly EcsArchetype[] Prior = new EcsArchetype[byte.MaxValue];


        private int _length;
        private int _count;
        private int _freeIndex = int.MaxValue;
        private EcsEntity[] _entities = new EcsEntity[1];
        private readonly IEcsComponentPool[] _compPools = new IEcsComponentPool[byte.MaxValue];

        public EcsArchetype(int id, byte[] indices)
        {
            Id = id;
            Indices = indices;
            SetIndices = new HashSet<byte>(indices);

            foreach (byte index in indices)
            {
                IEcsComponentPoolCreator creator = EcsTypeManager.ComponentPoolCreators[index];
                _compPools[index] = creator.InstantiatePool();
            }
        }
        
        public IEcsEntity this[int index]
        {
            get
            {
                if (index < 0 || index >= _length)
                    throw new IndexOutOfRangeException();
                
                RemoveHoles();
                return _entities[index];
            }
        }

        public EcsComponentPool<TC> GetComponentPool<TC>() where TC : class, IEcsComponent, new()
        {
            return (EcsComponentPool<TC>) _compPools[EcsComponentType<TC>.Index];
        }

        public IEcsComponentPool GetComponentPool(byte index)
        {
            return _compPools[index];
        }

        public EcsEntity[] GetEntities(out int length)
        {
            RemoveHoles();
            length = _length;
            return _entities;
        }

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
            _compPools[index].AddComponent(_length, component);
        }

        public void RemoveEntity(EcsEntity entity)
        {
            _entities[entity.ArchetypeIndex] = null;
            
            foreach (byte index in Indices)
            {
                _compPools[index].Remove(entity.ArchetypeIndex);
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

                foreach (byte index in Indices)
                {
                    _compPools[index].Replace(_freeIndex, current);
                }

                current++;
                _freeIndex++;
            }

            _length = _freeIndex;
            _freeIndex = int.MaxValue;
        }
    }
}