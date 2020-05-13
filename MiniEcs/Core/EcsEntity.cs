using System;

namespace MiniEcs.Core
{
    public interface IEcsEntity
    {
        uint Id { get; }
        bool HasComponent<T>() where T : IEcsComponent;
        T GetComponent<T>() where T : IEcsComponent;
        void AddComponent<T>(T component) where T : IEcsComponent;
        void RemoveComponent<T>() where T : IEcsComponent;
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

        /// <summary>
        /// Archetype Manager
        /// </summary>
        private readonly EcsArchetypeManager _archetypeManager;

        /// <summary>
        /// Creates a new entity, with an initial set of components.
        /// </summary>
        /// <param name="archetypeManager">Archetype Manager</param>
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

        public void Initialize<T0>(uint id, T0 component0) where T0 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0);
            _archetype.AddComponent(index0, component0);
            _archetype.AddEntity(this);
        }

        public void Initialize<T0, T1>(uint id, T0 component0, T1 component1)
            where T0 : IEcsComponent where T1 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            byte index1 = EcsComponentType<T1>.Index;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1);
            _archetype.AddComponent(index0, component0);
            _archetype.AddComponent(index1, component1);
            _archetype.AddEntity(this);
        }

        public void Initialize<T0, T1, T2>(uint id, T0 component0, T1 component1, T2 component2)
            where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            byte index1 = EcsComponentType<T1>.Index;
            byte index2 = EcsComponentType<T2>.Index;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1, index2);
            _archetype.AddComponent(index0, component0);
            _archetype.AddComponent(index1, component1);
            _archetype.AddComponent(index2, component2);
            _archetype.AddEntity(this);
        }

        public void Initialize<T0, T1, T2, T3>(uint id, T0 component0, T1 component1, T2 component2,
            T3 component3) where T0 : IEcsComponent
            where T1 : IEcsComponent
            where T2 : IEcsComponent
            where T3 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            byte index1 = EcsComponentType<T1>.Index;
            byte index2 = EcsComponentType<T2>.Index;
            byte index3 = EcsComponentType<T3>.Index;
            
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
        public bool HasComponent<T>() where T : IEcsComponent
        {
            return _archetype.SetIndices.Contains(EcsComponentType<T>.Index);
        }

        /// <summary>
        /// Get component by specified type
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <returns>Component</returns>
        /// <exception cref="InvalidOperationException">
        /// A component with the specified type does not exist
        /// </exception>
        public T GetComponent<T>() where T : IEcsComponent
        {
            T comp = _archetype.GetComponentPool<T>().GetTyped(ArchetypeIndex);
            if (comp == null)
                throw new InvalidOperationException();
            return comp;
        }

        /// <summary>
        /// Add component by specified type
        /// </summary>
        /// <param name="component">Component</param>
        /// <typeparam name="T">Component Type</typeparam>
        /// <exception cref="ArgumentException">
        /// A component with the specified type already exists OR component is NULL
        /// </exception>
        public void AddComponent<T>(T component) where T : IEcsComponent
        {
            if (HasComponent<T>() || component == null)
                throw new ArgumentException();
            
            byte index = EcsComponentType<T>.Index;
            EcsArchetype newArchetype = _archetypeManager.FindOrCreateNextArchetype(_archetype, index);
            
            for (int i = 0; i < _archetype.IndicesCount; i++)
            {
                byte ind = _archetype.Indices[i];
                IEcsComponentPool componentPool = _archetype.GetComponentPool(ind);
                newArchetype.AddComponent(ind, componentPool.Get(ArchetypeIndex));
            }
            newArchetype.AddComponent(index, component);

            _archetype.RemoveEntity(this);
            _archetype = newArchetype;
            _archetype.AddEntity(this);
        }

        /// <summary>
        /// Remove component by specified type
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        /// <exception cref="InvalidOperationException">
        /// A component with the specified type does not exist
        /// </exception>
        public void RemoveComponent<T>() where T : IEcsComponent
        {
            if (!HasComponent<T>())
                throw new InvalidOperationException();
            
            byte index = EcsComponentType<T>.Index;
            EcsArchetype newArchetype = _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
            
            for (int i = 0; i < _archetype.IndicesCount; i++)
            {
                byte ind = _archetype.Indices[i];
                if (ind == index)
                    continue;
                
                IEcsComponentPool componentPool = _archetype.GetComponentPool(ind);
                newArchetype.AddComponent(ind, componentPool.Get(ArchetypeIndex));
            }
            
            _archetype.RemoveEntity(this);
            _archetype = newArchetype;
            _archetype.AddEntity(this);
        }

        /// <summary>
        /// Destroy entity
        /// </summary>
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