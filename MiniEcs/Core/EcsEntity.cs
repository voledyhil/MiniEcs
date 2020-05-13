using System;

namespace MiniEcs.Core
{
    public interface IEcsEntity
    {
        uint Id { get; }
        bool HasComponent<TC>() where TC : IEcsComponent;
        TC GetComponent<TC>() where TC : IEcsComponent;
        void AddComponent<TC>(TC component) where TC : IEcsComponent;
        void RemoveComponent<TC>() where TC : IEcsComponent;
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

        public void Initialize<TC0>(uint id, TC0 component0) where TC0 : IEcsComponent
        {
            Id = id;

            byte index0 = EcsComponentType<TC0>.Index;

            _archetype = _archetypeManager.FindOrCreateArchetype(index0);
            _archetype.AddComponent(index0, component0);
            _archetype.AddEntity(this);
        }

        public void Initialize<TC0, TC1>(uint id, TC0 component0, TC1 component1)
            where TC0 : IEcsComponent where TC1 : IEcsComponent
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
            where TC0 : IEcsComponent where TC1 : IEcsComponent where TC2 : IEcsComponent
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

        public void Initialize<TC0, TC1, TC2, TC3>(uint id, TC0 component0, TC1 component1, TC2 component2, TC3 component3)
            where TC0 : IEcsComponent where TC1 : IEcsComponent where TC2 : IEcsComponent where TC3 : IEcsComponent
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
        public bool HasComponent<TC>() where TC : IEcsComponent
        {
            return _archetype.SetIndices.Contains(EcsComponentType<TC>.Index);
        }

        /// <summary>
        /// Get component by specified type
        /// </summary>
        /// <returns>Component</returns>
        /// <exception cref="InvalidOperationException">
        /// A component with the specified type does not exist
        /// </exception>
        public TC GetComponent<TC>() where TC : IEcsComponent
        {
            if (!HasComponent<TC>())
                throw new InvalidOperationException();

            return _archetype.GetComponentPool<TC>().GetTyped(ArchetypeIndex);
        }

        /// <summary>
        /// Add component by specified type
        /// </summary>
        /// <param name="component">Component</param>
        /// <exception cref="ArgumentException">
        /// A component with the specified type already exists OR component is NULL
        /// </exception>
        public void AddComponent<TC>(TC component) where TC : IEcsComponent
        {
            if (HasComponent<TC>() || component == null)
                throw new ArgumentException();

            byte index = EcsComponentType<TC>.Index;
            EcsArchetype newArchetype = _archetypeManager.FindOrCreateNextArchetype(_archetype, index);

            for (int i = 0; i < _archetype.IndicesCount; i++)
            {
                byte curIndex = _archetype.Indices[i];
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
        public void RemoveComponent<TC>() where TC : IEcsComponent
        {
            if (!HasComponent<TC>())
                throw new InvalidOperationException();

            byte index = EcsComponentType<TC>.Index;
            EcsArchetype newArchetype = _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);

            for (int i = 0; i < _archetype.IndicesCount; i++)
            {
                byte curIndex = _archetype.Indices[i];
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