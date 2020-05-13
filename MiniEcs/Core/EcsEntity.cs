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
        /// Сollection of components. Memory is allocated for all possible types of
        /// unique components, for quick access by index
        /// </summary>
        private readonly IEcsComponent[] _components;

        /// <summary>
        /// Creates a new entity, with an initial set of components.
        /// </summary>
        /// <param name="archetypeManager">Archetype Manager</param>
        protected EcsEntity(EcsArchetypeManager archetypeManager)
        {
            _components = new IEcsComponent[byte.MaxValue];
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
            
            _components[index0] = component0;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0);
            _archetype.AddEntity(this);
        }

        public void Initialize<T0, T1>(uint id, T0 component0, T1 component1)
            where T0 : IEcsComponent where T1 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            byte index1 = EcsComponentType<T1>.Index;
            
            _components[index0] = component0;
            _components[index1] = component1;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1);
            _archetype.AddEntity(this);
        }

        public void Initialize<T0, T1, T2>(uint id, T0 component0, T1 component1, T2 component2)
            where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            Id = id;
            
            byte index0 = EcsComponentType<T0>.Index;
            byte index1 = EcsComponentType<T1>.Index;
            byte index2 = EcsComponentType<T2>.Index;
            
            _components[index0] = component0;
            _components[index1] = component1;
            _components[index2] = component2;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1, index2);
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
            
            _components[index0] = component0;
            _components[index1] = component1;
            _components[index2] = component2;
            _components[index3] = component3;
            
            _archetype = _archetypeManager.FindOrCreateArchetype(index0, index1, index2, index3);
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
            return _components[EcsComponentType<T>.Index] != null;
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
            IEcsComponent comp = _components[EcsComponentType<T>.Index];
            if (comp == null)
                throw new InvalidOperationException();
            return (T) comp;
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
            byte index = EcsComponentType<T>.Index;
            IEcsComponent comp = _components[index];

            if (!(comp == null && component != null))
                throw new ArgumentException();

            _archetype.RemoveEntity(this);
            _archetype = _archetypeManager.FindOrCreateNextArchetype(_archetype, index);
            _archetype.AddEntity(this);

            _components[index] = component;
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
            byte index = EcsComponentType<T>.Index;
            IEcsComponent component = _components[index];

            if (component == null)
                throw new InvalidOperationException();

            _archetype.RemoveEntity(this);
            _archetype = _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
            _archetype.AddEntity(this);

            _components[index] = null;
        }

        /// <summary>
        /// Destroy entity
        /// </summary>
        public void Destroy()
        {
            foreach (byte index in _archetype.Indices)
            {
                _components[index] = null;
            }

            _archetype.RemoveEntity(this);
            _archetype = null;

            OnDestroy();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}