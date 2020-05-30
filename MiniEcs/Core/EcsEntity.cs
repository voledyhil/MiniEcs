using System;

namespace MiniEcs.Core
{
    public interface IEcsEntity
    {
        uint Id { get; }
        int ComponentsCount { get; }
        bool HasComponent<TC>() where TC : class, IEcsComponent, new();
        TC GetComponent<TC>() where TC : class, IEcsComponent, new();
        void AddComponent<TC>(TC component) where TC : class, IEcsComponent, new();
        void RemoveComponent<TC>() where TC : class, IEcsComponent, new();
        void Destroy();
    }

    /// <summary>
    /// An entity is a collection of unique components
    /// </summary>
    public abstract class EcsEntity : IEcsEntity
    {
        /// <summary>
        /// Position within the archetype
        /// </summary>
        public int ArchetypeIndex;

        /// <summary>
        /// Unique identifier of an entity
        /// </summary>
        public uint Id { get; private set; }

        public int ComponentsCount => _archetype.Indices.Length;

        /// <summary>
        /// Current Entity Archetype
        /// </summary>
        private EcsArchetype _archetype;

        private readonly EcsArchetypeManager _archetypeManager;

        protected EcsEntity(EcsArchetypeManager archetypeManager)
        {
            _archetypeManager = archetypeManager;
        }

        public void Initialize(uint id)
        {
            Id = id;
            _archetype = _archetypeManager.Empty;
            _archetype.AddEntity(this);
        }

        public void Initialize<TC0>(uint id, TC0 component0) where TC0 : class, IEcsComponent, new()
        {
            Id = id;

            byte index0 = EcsComponentType<TC0>.Index;

            _archetype = _archetypeManager.FindOrCreateArchetype(index0);
            _archetype.AddComponent(index0, component0);
            _archetype.AddEntity(this);
        }

        public void Initialize<TC0, TC1>(uint id, TC0 component0, TC1 component1)
            where TC0 : class, IEcsComponent, new() 
            where TC1 : class, IEcsComponent, new()
        {
            Id = id;

            byte index0 = EcsComponentType<TC0>.Index;
            byte index1 = EcsComponentType<TC1>.Index;

            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1);
            _archetype.AddComponent(index0, component0);
            _archetype.AddComponent(index1, component1);
            _archetype.AddEntity(this);
        }

        public void Initialize<TC0, TC1, TC2>(uint id, TC0 component0, TC1 component1, TC2 component2)
            where TC0 : class, IEcsComponent, new()
            where TC1 : class, IEcsComponent, new()
            where TC2 : class, IEcsComponent, new()
        {
            Id = id;

            byte index0 = EcsComponentType<TC0>.Index;
            byte index1 = EcsComponentType<TC1>.Index;
            byte index2 = EcsComponentType<TC2>.Index;

            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1, index2);
            _archetype.AddComponent(index0, component0);
            _archetype.AddComponent(index1, component1);
            _archetype.AddComponent(index2, component2);
            _archetype.AddEntity(this);
        }

        public void Initialize<TC0, TC1, TC2, TC3>(uint id, TC0 component0, TC1 component1, TC2 component2,
            TC3 component3)
            where TC0 : class, IEcsComponent, new()
            where TC1 : class, IEcsComponent, new()
            where TC2 : class, IEcsComponent, new()
            where TC3 : class, IEcsComponent, new()
        {
            Id = id;

            byte index0 = EcsComponentType<TC0>.Index;
            byte index1 = EcsComponentType<TC1>.Index;
            byte index2 = EcsComponentType<TC2>.Index;
            byte index3 = EcsComponentType<TC3>.Index;

            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1, index2, index3);
            _archetype.AddComponent(index0, component0);
            _archetype.AddComponent(index1, component1);
            _archetype.AddComponent(index2, component2);
            _archetype.AddComponent(index3, component3);
            _archetype.AddEntity(this);
        }



        /// <summary>
        /// Determines whether the specified component type is contained in an entity.
        /// </summary>
        /// <returns>
        /// true if the entity contains a component of the specified type; otherwise, false.
        /// </returns>
        public bool HasComponent<TC>() where TC : class, IEcsComponent, new()
        {
            return HasComponent(EcsComponentType<TC>.Index);
        }

        internal bool HasComponent(byte index)
        {
            return _archetype.SetIndices.Contains(index);
        }

        /// <summary>
        /// Get component by specified type
        /// </summary>
        /// <returns>Component</returns>
        /// <exception cref="InvalidOperationException">
        /// A component with the specified type does not exist
        /// </exception>
        public TC GetComponent<TC>() where TC : class, IEcsComponent, new()
        {
            if (!HasComponent<TC>())
                throw new InvalidOperationException();

            return _archetype.GetComponentPool<TC>().GetTyped(ArchetypeIndex);
        }

        internal IEcsComponent GetComponent(byte index)
        {
            return _archetype.GetComponentPool(index).Get(ArchetypeIndex);
        }

        /// <summary>
        /// Add component by specified type
        /// </summary>
        /// <param name="component">Component</param>
        /// <exception cref="ArgumentException">
        /// A component with the specified type already exists OR component is NULL
        /// </exception>
        public void AddComponent<TC>(TC component) where TC : class, IEcsComponent, new()
        {
            byte index = EcsComponentType<TC>.Index;
            if (HasComponent(index) || component == null)
                throw new ArgumentException();
            AddComponent(index, component);
        }

        internal void AddComponent(byte index, IEcsComponent component)
        {
            EcsArchetype newArchetype = _archetypeManager.FindOrCreateNextArchetype(_archetype, index);
            foreach (byte curIndex in _archetype.Indices)
            {
                IEcsComponentPool componentPool = _archetype.GetComponentPool(curIndex);
                newArchetype.AddComponent(curIndex, componentPool.Get(ArchetypeIndex));
            }

            newArchetype.AddComponent(index, component);

            _archetype.RemoveEntity(this);
            _archetype = newArchetype;
            _archetype.AddEntity(this);
        }

        /// <summary>
        /// Remove component by specified type
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// A component with the specified type does not exist
        /// </exception>
        public void RemoveComponent<TC>() where TC : class, IEcsComponent, new()
        {
            byte index = EcsComponentType<TC>.Index;
            if (!HasComponent(index))
                throw new InvalidOperationException();
            RemoveComponent(index);
        }

        internal void RemoveComponent(byte index)
        {
            EcsArchetype newArchetype = _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
            foreach (byte curIndex in _archetype.Indices)
            {
                if (curIndex == index)
                    continue;

                IEcsComponentPool componentPool = _archetype.GetComponentPool(curIndex);
                newArchetype.AddComponent(curIndex, componentPool.Get(ArchetypeIndex));
            }

            _archetype.RemoveEntity(this);
            _archetype = newArchetype;
            _archetype.AddEntity(this);
        }

        public void Destroy()
        {
            _archetype.RemoveEntity(this);
            _archetype = null;

            OnDestroy();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}