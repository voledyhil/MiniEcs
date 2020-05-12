using System;

namespace MiniEcs.Core
{
    /// <summary>
    /// An entity is a collection of unique components
    /// </summary>
    public abstract class EcsEntity
    {
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

        protected void InnerInitialize(uint id)
        {
            Id = id;
            _archetype = _archetypeManager.Empty;
            _archetype.Entities.Add(this);
        }

        protected void InnerInitialize<T0>(uint id, T0 component0) where T0 : IEcsComponent
        {
            InnerInitialize(id);

            AddComponent(component0);
        }

        protected void InnerInitialize<T0, T1>(uint id, T0 component0, T1 component1)
            where T0 : IEcsComponent where T1 : IEcsComponent
        {
            InnerInitialize(id);

            AddComponent(component0);
            AddComponent(component1);
        }

        protected void InnerInitialize<T0, T1, T2>(uint id, T0 component0, T1 component1, T2 component2)
            where T0 : IEcsComponent where T1 : IEcsComponent where T2 : IEcsComponent
        {
            InnerInitialize(id);

            AddComponent(component0);
            AddComponent(component1);
            AddComponent(component2);
        }

        protected void InnerInitialize<T0, T1, T2, T3>(uint id, T0 component0, T1 component1, T2 component2,
            T3 component3) where T0 : IEcsComponent
            where T1 : IEcsComponent
            where T2 : IEcsComponent
            where T3 : IEcsComponent
        {
            InnerInitialize(id);

            AddComponent(component0);
            AddComponent(component1);
            AddComponent(component2);
            AddComponent(component3);
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

            _archetype.Entities.Remove(this);
            _archetype = _archetypeManager.FindOrCreateNextArchetype(_archetype, index);
            _archetype.Entities.Add(this);

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

            _archetype.Entities.Remove(this);
            _archetype = _archetypeManager.FindOrCreatePriorArchetype(_archetype, index);
            _archetype.Entities.Add(this);

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

            _archetype.Entities.Remove(this);
            _archetype = null;

            OnDestroy();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}