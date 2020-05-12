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
        /// Calculate number entities in archetype
        /// </summary>
        public int EntitiesCount
        {
            get
            {
                if (!_needRemoveHoles)
                    return _length;

                RemoveHoles();
                _needRemoveHoles = false;
                return _length;
            }
        }

        /// <summary>
        /// Number entities in archetype
        /// </summary>
        private int _length;

        /// <summary>
        /// There are holes in the array, you need to rebuild the entities
        /// </summary>
        private bool _needRemoveHoles;

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
        private EcsEntity[] _entities = new EcsEntity[1];

        /// <summary>
        /// Transitions to the next archetype when adding a new type of component
        /// </summary>
        public readonly Dictionary<byte, EcsArchetype> Next = new Dictionary<byte, EcsArchetype>();

        /// <summary>
        /// Transitions to the previous archetype when deleting an existing component type
        /// </summary>
        public readonly Dictionary<byte, EcsArchetype> Prior = new Dictionary<byte, EcsArchetype>();

        /// <summary>
        /// Returns all entities of a given archetype
        /// </summary>
        /// <param name="length">Number entities in archetype</param>
        /// <returns>Entities array</returns>
        public EcsEntity[] GetEntities(out int length)
        {
            if (_needRemoveHoles)
            {
                RemoveHoles();
                _needRemoveHoles = false;
            }

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
        }

        /// <summary>
        /// Remove entity from archetype
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveEntity(EcsEntity entity)
        {
            _entities[entity.ArchetypeIndex] = null;
            _needRemoveHoles = true;
        }

        /// <summary>
        /// Remove holes from the archetype
        /// </summary>
        private void RemoveHoles()
        {
            int freeIndex = int.MaxValue;
            for (int i = 0; i < _length; i++)
            {
                if (_entities[i] != null)
                    continue;

                freeIndex = i;
                break;
            }

            if (freeIndex >= _length)
                return;

            int current = freeIndex + 1;
            while (current < _length)
            {
                while (current < _length && _entities[current] == null)
                    current++;

                if (current >= _length)
                    continue;

                EcsEntity entity = _entities[current];
                entity.ArchetypeIndex = freeIndex;

                _entities[freeIndex] = entity;

                freeIndex++;
                current++;
            }

            _length = freeIndex;
        }
    }
}